import sys, glob, os
import numpy as np
from PIL import Image
from scipy import ndimage

THR = 236  # near-white threshold

def cut(arr):
    a = arr.copy()
    rgb = a[:, :, :3].astype(np.int32)
    white = (rgb[:, :, 0] >= THR) & (rgb[:, :, 1] >= THR) & (rgb[:, :, 2] >= THR)
    lbl, n = ndimage.label(white)
    border = set(lbl[0, :]).union(lbl[-1, :]).union(lbl[:, 0]).union(lbl[:, -1])
    border.discard(0)
    bg = np.isin(lbl, list(border))
    a[:, :, 3] = np.where(bg, 0, a[:, :, 3])
    return a

def preview(a, bg=(255, 0, 255)):
    h, w = a.shape[:2]
    canvas = np.zeros((h, w, 3), np.uint8)
    canvas[:, :] = bg
    alpha = a[:, :, 3:4].astype(np.float32) / 255.0
    out = (a[:, :, :3].astype(np.float32) * alpha + canvas * (1 - alpha)).astype(np.uint8)
    return Image.fromarray(out)

mode = sys.argv[1]
if mode == "test":
    outdir = sys.argv[2]
    for p in sys.argv[3:]:
        im = Image.open(p).convert("RGBA")
        a = cut(np.array(im))
        name = os.path.splitext(os.path.basename(p))[0]
        Image.fromarray(a).save(os.path.join(outdir, name + "_cut.png"))
        preview(a).save(os.path.join(outdir, name + "_preview.png"))
        print("test", name)
elif mode == "run":
    root = sys.argv[2]
    files = glob.glob(os.path.join(root, "**", "*.png"), recursive=True)
    files = [f for f in files if not f.endswith("_cut.png") and not f.endswith("_preview.png")]
    for p in files:
        im = Image.open(p).convert("RGBA")
        a = cut(np.array(im))
        Image.fromarray(a).save(p)
    print("processed", len(files), "files in", root)
