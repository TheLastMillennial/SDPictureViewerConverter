from PIL import Image
import numpy as np
import sys
from tivars.models import TI_84PCE
from tivars.tokenizer import *
from tivars.types import *
from pathlib import Path


# === Setup ===

# Map the 15 calculator colors to the correct index
color_to_index = {
    (0, 0, 255): 10,
    (255, 0, 0): 11,
    (0, 0, 0): 12,
    (255, 0, 255): 13,
    (0, 159, 0): 14,
    (255, 143, 32): 15,
    (182, 32, 0): 16,
    (0, 0, 134): 17,
    (0, 147, 255): 18,
    (255, 255, 0): 19,
    (255, 255, 255): 20,
    (231, 226, 231): 21,
    (199, 195, 199): 22,
    (143, 139, 143): 23,
    (81, 85, 81): 24
}

palette = list(color_to_index.keys())

def closest_color(pixel, palette):
    return min(palette, key=lambda c: sum((p - q) ** 2 for p, q in zip(pixel, c)))

def floyd_steinberg_dither(image, palette):
    img = np.array(image, dtype=np.float32)
    for y in range(img.shape[0]):
        for x in range(img.shape[1]):
            old_pixel = img[y, x].copy()
            new_pixel = closest_color(old_pixel, palette)
            img[y, x] = new_pixel
            error = old_pixel - new_pixel

            if x + 1 < img.shape[1]:
                img[y, x + 1] += error * 7 / 16
            if x - 1 >= 0 and y + 1 < img.shape[0]:
                img[y + 1, x - 1] += error * 3 / 16
            if y + 1 < img.shape[0]:
                img[y + 1, x] += error * 5 / 16
            if x + 1 < img.shape[1] and y + 1 < img.shape[0]:
                img[y + 1, x + 1] += error * 1 / 16

    img = np.clip(img, 0, 255)
    return Image.fromarray(img.astype(np.uint8))

# Convert a picture to only use the colors the calculator has.
def process_image_to_index_list(input_path):
    image = Image.open(input_path).convert("RGB")
    image = image.resize((265, 165), Image.LANCZOS)
    dithered = floyd_steinberg_dither(image, palette)
    dithered.save("output.png")

    # Convert to list of indices
    pixels = np.array(dithered)
    indexed_pixels=[]
    for row in pixels:
        for pixel in row:
            indexed_pixels.append(color_to_index[tuple(pixel)])

    return indexed_pixels

# Convert the list of pixel color values to a TI List
def save_as_tireallist(indexed_list, var_name : str):
    # Use first 4 chars of picture name for list name
    if len(var_name) > 4:
        var_name = var_name[:4]

    # concatenate numbers into a list of floats to save space
    concatenated_list = concatenate_to_floats(indexed_list)
    chunks = [concatenated_list[x:x+999] for x in range(0, len(concatenated_list), 999)]

    for i,cur_list in enumerate( chunks):
        ti_list = [TIReal(str(num)) for num in cur_list]
        
        fullVarName=var_name+str(i)
        ti_real_list = TIRealList(name=(fullVarName))
        ti_real_list.load_list(ti_list)

        fullFilename = "List_for_"+var_name+str(i)+".8xl"
        ti_real_list.save(fullFilename)

        print(f'Generated list: {fullFilename}')
        #print(f"Saved {len(cur_list)} entries to '{fullFilename}' as list '{fullVarName}'")

def generate_ti_program(var_name:str):
    # Use first 4 chars of picture name for prgm name
    if len(var_name) > 4:
        var_name = var_name[:4]
    var_name=var_name.upper()
   
    my_program = TIProgram(name=("DRAW"+str(var_name)))
    code: str = "ClrDraw\n⁻1→X\n0→Y\n\nSetUpEditor L₁,"+var_name+"0\nSetUpEditor "+var_name+"1,"+var_name+"2,"+var_name+"3,"+var_name+"4,"+var_name+"5,"+var_name+"6\n\nFor(L,0,6\nIf L=0\nʟ"+var_name+"0→L₁\nIf L=1\nʟ"+var_name+"1→L₁\nIf L=2\nʟ"+var_name+"2→L₁\nIf L=3\nʟ"+var_name+"3→L₁\nIf L=4\nʟ"+var_name+"4→L₁\nIf L=5\nʟ"+var_name+"5→L₁\nIf L=6\nʟ"+var_name+"6→L₁\n\nFor(I,1,dim(L₁\nL₁(I→B\n\nFor(J,0,6\n1+X→X\nIf Ans>264\nThen\nY+1→Y\nIf Ans>164\nGoto Z\n0→X\nEnd\n\nPxl-On(Y,Ans,iPart(B\nfPart(B)ᴇ2→B\n\nEnd\nEnd\nEnd\n\nLbl Z\nDelVar L₁\nArchive ʟ"+var_name+"0\nArchive ʟ"+var_name+"1\nArchive ʟ"+var_name+"2\nArchive ʟ"+var_name+"3\nArchive ʟ"+var_name+"4\nArchive ʟ"+var_name+"5\nArchive ʟ"+var_name+"5\nArchive ʟ"+var_name+"6"
    my_program.load_string(code, model=TI_84PCE)    # Errors if the code isn't supported by the TI-84+CE
    
    fullFilename = "DRAW"+var_name+".8xp"
    my_program.save(fullFilename)

    print(f'Generated program: {fullFilename}')

# turn a list of numbers like [10,11,12,13,14] into [10.11121314]
def concatenate_to_floats(nums):
    floats = []
    for i in range(0, len(nums), 7):
        chunk = nums[i:i+7]
        # Format each number to 2 digits
        joined = ''.join(f"{n:02d}" for n in chunk)
        # Put a decimal after the first two digits
        float_str = f"{joined[:2]}.{joined[2:]}" if len(joined) > 2 else joined
        floats.append(float(float_str))
    return floats

if __name__=="__main__":
    print("SD Picture Viewer Converter by TheLastMillennial")
    print("GitHub: https://github.com/TheLastMillennial/SDPictureViewerConverter\n")

    picNameWithExtension:str=""
    if len(sys.argv) > 1:
        picNameWithExtension = sys.argv[1]
    else:
        picNameWithExtension=input("Drag and drop the picture to convert here. Then press enter: ")
    picName = Path(picNameWithExtension).stem

    print("\nProcessing picture...")
    indexed_list = process_image_to_index_list(picNameWithExtension)
    print("Generating calculator files...\n")
    save_as_tireallist(indexed_list, picName)
    generate_ti_program(picName)
    print("\nFinished. Send all generated files to your TI-84 Plus CE.")
