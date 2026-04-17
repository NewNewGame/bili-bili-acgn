/**
 * AtlasFromFolder.jsx
 * Photoshop (ExtendScript): pack equal-sized images from a folder into atlas PNGs (max 4096×4096 each)
 * and emit one .tpsheet JSON compatible with Godot / TexturePacker-style loaders (see card_atlas.tpsheet).
 *
 * Usage: File > Scripts > Browse… → select this file.
 *
 * Output in chosen folder:
 *   {baseName}.tpsheet
 *   {baseName}_0.png, {baseName}_1.png, …
 *
 * Recurses subfolders. "filename" in JSON uses relative paths with "/" (e.g. sub/card.png).
 */

#target photoshop

app.displayDialogs = DialogModes.NO;

var MAX_ATLAS = 4096;
var EDGE_PAD = 0;
var CELL_GAP = 0;
var CANDIDATE_PAGE_SIZES = [256, 512, 1024, 2048, 4096];
var DEFAULT_NAME_PREFIX = "bilibiliacgn_";

function main() {
    var srcFolder = Folder.selectDialog("选择包含图片的文件夹（只扫描当前文件夹，不进入子文件夹）");
    if (!srcFolder) return;

    var outFolder = Folder.selectDialog("选择输出文件夹（将写入 .tpsheet 与 PNG）");
    if (!outFolder) return;

    var baseName = prompt("图集基础文件名（不含扩展名）", DEFAULT_NAME_PREFIX + "card_atlas");
    if (baseName === null) return;
    baseName = trim_(baseName);
    if (!baseName.length) {
        alert("基础文件名不能为空。");
        return;
    }
    if (baseName.indexOf(DEFAULT_NAME_PREFIX) !== 0) {
        baseName = DEFAULT_NAME_PREFIX + baseName;
    }

    var items = [];
    collectImagesFlat_(srcFolder, "", items);
    if (!items.length) {
        alert("未找到 PNG / JPG / JPEG 图片。");
        return;
    }
    // Read each image's size (supports up to 2 distinct sizes).
    var sizeMap = {}; // key => {w,h,count}
    try {
        for (var i = 0; i < items.length; i++) {
            var d = app.open(items[i].file);
            try {
                var w = d.width.value;
                var h = d.height.value;
                items[i].w = w;
                items[i].h = h;
                var key = String(w) + "x" + String(h);
                if (!sizeMap[key]) sizeMap[key] = { w: w, h: h, count: 0 };
                sizeMap[key].count += 1;
            } finally {
                d.close(SaveOptions.DONOTSAVECHANGES);
            }
        }
    } catch (e) {
        alert("打开图片失败：\n" + e.message);
        return;
    }

    var sizes = [];
    for (var k in sizeMap) if (sizeMap.hasOwnProperty(k)) sizes.push(sizeMap[k]);
    if (sizes.length > 2) {
        var msg = "最多只允许两种不同尺寸的图片（当前发现 " + sizes.length + " 种）：\n";
        for (var si = 0; si < sizes.length; si++) {
            msg += "- " + sizes[si].w + "×" + sizes[si].h + " (" + sizes[si].count + ")\n";
        }
        alert(msg);
        return;
    }

    // Ensure each sprite fits within a single 4096 page (accounting for padding).
    for (var ii = 0; ii < items.length; ii++) {
        if (items[ii].w + EDGE_PAD > MAX_ATLAS || items[ii].h + EDGE_PAD > MAX_ATLAS) {
            alert(
                "单张图片（含 " +
                    EDGE_PAD +
                    "px 边距）必须不超过 " +
                    MAX_ATLAS +
                    " 像素宽高：\n" +
                    items[ii].rel +
                    " (" +
                    items[ii].w +
                    "×" +
                    items[ii].h +
                    ")"
            );
            return;
        }
    }

    // Packing order: larger first, then deterministic by path.
    items.sort(function (a, b) {
        if (a.h !== b.h) return b.h - a.h;
        if (a.w !== b.w) return b.w - a.w;
        return a.rel.toLowerCase().localeCompare(b.rel.toLowerCase());
    });

    var pageSize = choosePageSize_(items);
    if (!pageSize) {
        alert("无法选择合适的方形图集尺寸（请检查是否存在超大图片）。");
        return;
    }

    var pages = [];
    var page = newPage_();
    var cx = EDGE_PAD;
    var cy = EDGE_PAD;
    var rowH = 0;

    for (var j = 0; j < items.length; j++) {
        var iw = items[j].w;
        var ih = items[j].h;

        // New row if doesn't fit horizontally.
        if (cx + iw > pageSize) {
            cx = EDGE_PAD;
            cy += rowH + CELL_GAP;
            rowH = 0;
        }

        // New page if doesn't fit vertically (at current row position).
        if (cy + ih > pageSize) {
            pages.push(page);
            page = newPage_();
            cx = EDGE_PAD;
            cy = EDGE_PAD;
            rowH = 0;
        }

        // Retry new row on a fresh page if still doesn't fit horizontally (very wide sprites).
        if (cx + iw > pageSize) {
            cx = EDGE_PAD;
            cy += rowH + CELL_GAP;
            rowH = 0;
        }

        page.sprites.push({
            file: items[j].file,
            rel: items[j].rel,
            x: cx,
            y: cy,
            w: iw,
            h: ih,
        });
        page.maxX = Math.max(page.maxX, cx + iw);
        page.maxY = Math.max(page.maxY, cy + ih);
        rowH = Math.max(rowH, ih);

        cx += iw + CELL_GAP;
    }
    pages.push(page);

    var textureBlocks = [];
    for (var p = 0; p < pages.length; p++) {
        var pngName = baseName + "_" + p + ".png";
        var pngFile = new File(outFolder.fsName + "/" + pngName);
        var dims = buildAtlasPage_(pages[p], pngFile, pngName, pageSize);
        textureBlocks.push({
            image: pngName,
            w: dims.w,
            h: dims.h,
            sprites: pages[p].sprites,
        });
    }

    var tpsheetFile = new File(outFolder.fsName + "/" + baseName + ".tpsheet");
    writeTpsheet_(tpsheetFile, textureBlocks);

    alert(
        "完成。\n页数：" +
            pages.length +
            "\n输出目录：\n" +
            outFolder.fsName
    );
}

function newPage_() {
    return { sprites: [], maxX: EDGE_PAD, maxY: EDGE_PAD };
}

function choosePageSize_(items) {
    var maxW = 0;
    var maxH = 0;
    var totalArea = 0;
    for (var i = 0; i < items.length; i++) {
        maxW = Math.max(maxW, items[i].w + EDGE_PAD);
        maxH = Math.max(maxH, items[i].h + EDGE_PAD);
        totalArea += items[i].w * items[i].h;
    }

    // Prefer square sizes; primary objective is fewest pages, then least wasted pixels.
    var best = null;
    for (var c = 0; c < CANDIDATE_PAGE_SIZES.length; c++) {
        var s = CANDIDATE_PAGE_SIZES[c];
        if (s > MAX_ATLAS) continue;
        if (s < maxW || s < maxH) continue;

        var sim = simulatePack_(items, s);
        if (!sim.ok) continue;

        var pages = sim.pages;
        var waste = pages * s * s - totalArea;
        if (best === null) {
            best = { size: s, pages: pages, waste: waste };
            continue;
        }
        if (pages < best.pages || (pages === best.pages && waste < best.waste)) {
            best = { size: s, pages: pages, waste: waste };
        }
    }
    return best ? best.size : null;
}

function simulatePack_(items, pageSize) {
    var pages = 1;
    var cx = EDGE_PAD;
    var cy = EDGE_PAD;
    var rowH = 0;

    for (var j = 0; j < items.length; j++) {
        var iw = items[j].w;
        var ih = items[j].h;

        if (iw + EDGE_PAD > pageSize || ih + EDGE_PAD > pageSize) {
            return { ok: false, pages: 0 };
        }

        if (cx + iw > pageSize) {
            cx = EDGE_PAD;
            cy += rowH + CELL_GAP;
            rowH = 0;
        }

        if (cy + ih > pageSize) {
            pages += 1;
            cx = EDGE_PAD;
            cy = EDGE_PAD;
            rowH = 0;
        }

        if (cx + iw > pageSize) {
            cx = EDGE_PAD;
            cy += rowH + CELL_GAP;
            rowH = 0;
        }

        rowH = Math.max(rowH, ih);
        cx += iw + CELL_GAP;
    }

    return { ok: true, pages: pages };
}

function collectImagesFlat_(folder, relPrefix, out) {
    var list = folder.getFiles();
    for (var i = 0; i < list.length; i++) {
        var f = list[i];
        if (f instanceof File && isImageFile_(f)) {
            out.push({ file: f, rel: relPrefix + f.name });
        }
    }
}

function isImageFile_(f) {
    var n = f.name.toLowerCase();
    return n.match(/\.(png|jpg|jpeg)$/);
}

function trim_(s) {
    return s.replace(/^\s+|\s+$/g, "");
}

function escapeJson_(s) {
    return String(s)
        .replace(/\\/g, "\\\\")
        .replace(/"/g, '\\"')
        .replace(/\r/g, "\\r")
        .replace(/\n/g, "\\n");
}

function writeTpsheet_(file, textureBlocks) {
    var sb = [];
    sb.push("{");
    sb.push('  "textures": [');
    for (var t = 0; t < textureBlocks.length; t++) {
        var tb = textureBlocks[t];
        sb.push("    {");
        sb.push('      "image": "' + escapeJson_(tb.image) + '",');
        sb.push('      "size": {');
        sb.push('        "w": ' + tb.w + ",");
        sb.push('        "h": ' + tb.h);
        sb.push("      },");
        sb.push('      "sprites": [');
        for (var s = 0; s < tb.sprites.length; s++) {
            var sp = tb.sprites[s];
            var fn = sp.rel.replace(/\\/g, "/");
            var comma = s < tb.sprites.length - 1 ? "," : "";
            sb.push("        {");
            sb.push('          "filename": "' + escapeJson_(fn) + '",');
            sb.push('          "region": {');
            sb.push('            "x": ' + sp.x + ",");
            sb.push('            "y": ' + sp.y + ",");
            sb.push('            "w": ' + sp.w + ",");
            sb.push('            "h": ' + sp.h);
            sb.push("          },");
            sb.push('          "margin": {');
            sb.push('            "x": 0,');
            sb.push('            "y": 0,');
            sb.push('            "w": 0,');
            sb.push('            "h": 0');
            sb.push("          }");
            sb.push("        }" + comma);
        }
        sb.push("      ]");
        sb.push("    }" + (t < textureBlocks.length - 1 ? "," : ""));
    }
    sb.push("  ]");
    sb.push("}");
    file.encoding = "UTF-8";
    file.open("w");
    file.write(sb.join("\n"));
    file.close();
}

function buildAtlasPage_(page, pngFile, pngName, pageSize) {
    // Keep the atlas PNG square for better packing consistency.
    var aw = pageSize;
    var ah = pageSize;

    var atlas = app.documents.add(
        aw,
        ah,
        72,
        pngName,
        NewDocumentMode.RGB,
        DocumentFill.TRANSPARENT
    );

    app.activeDocument = atlas;

    try {
        for (var i = 0; i < page.sprites.length; i++) {
            var sp = page.sprites[i];
            var srcDoc = app.open(sp.file);
            try {
                app.activeDocument = srcDoc;
                srcDoc.selection.selectAll();
                srcDoc.selection.copy();
            } finally {
                srcDoc.close(SaveOptions.DONOTSAVECHANGES);
            }

            app.activeDocument = atlas;
            atlas.selection.select([
                [sp.x, sp.y],
                [sp.x + sp.w, sp.y],
                [sp.x + sp.w, sp.y + sp.h],
                [sp.x, sp.y + sp.h],
            ]);
            atlas.paste();
            try {
                var lyr = atlas.activeLayer;
                var b = lyr.bounds;
                var dx = sp.x - b[0].value;
                var dy = sp.y - b[1].value;
                if (dx !== 0 || dy !== 0) {
                    lyr.translate(
                        new UnitValue(dx, "px"),
                        new UnitValue(dy, "px")
                    );
                }
                lyr.name = "s_" + i;
            } catch (eAlign) {}
            try {
                atlas.selection.deselect();
            } catch (eDes) {}
        }

        var pngOpts = new PNGSaveOptions();
        pngOpts.compression = 6;
        pngOpts.interlaced = false;
        atlas.saveAs(pngFile, pngOpts, true);
    } finally {
        atlas.close(SaveOptions.DONOTSAVECHANGES);
    }

    return { w: aw, h: ah };
}

main();
