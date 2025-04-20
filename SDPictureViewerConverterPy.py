from PIL import Image
import numpy as np
from tivars.models import TI_84PCE
#from tivars import *
from tivars.tokenizer import *
from tivars.types import *

# === Setup ===

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
           # print(str(tuple(pixel)) + " " + str ( color_to_index[tuple(pixel)]))
            indexed_pixels.append(color_to_index[tuple(pixel)])

    #indexed_pixels = [
    #    color_to_index[tuple(pixel)]
    #    for row in pixels for pixel in row
    #]
    return indexed_pixels

def save_as_tireallist(indexed_list, var_name="PIC", filename="output"):
    if len(var_name) >4:
        var_name = var_name[:3]
    # concatenate numbers into a list of floats to save space
    concatenated_list = concatenate_to_floats(indexed_list)
    chunks = [concatenated_list[x:x+999] for x in range(0, len(concatenated_list), 999)]

    for i,cur_list in enumerate( chunks):
        ti_list = [TIReal(str(num)) for num in cur_list]
        
        fullVarName=var_name+str(i)
        ti_real_list = TIRealList(name=(fullVarName))
        ti_real_list.load_list(ti_list)

        fullFilename = filename+str(i)+".8xl"
        ti_real_list.save(fullFilename)

        print(f"Saved {len(cur_list)} entries to '{fullFilename}' as list '{fullVarName}'")

def concatenate_to_floats(nums):
    """
    floats = []
    chunk_size = 7
    for i in range(0, len(numbers), chunk_size):
        chunk = numbers[i:i + chunk_size]
        # Pad with zeros if the chunk is too short
        if len(chunk) < chunk_size:
            chunk += [0] * (chunk_size - len(chunk))
        # Convert each number to a 2-digit string
        str_chunk = ''.join(f"{n:02}" for n in chunk)
        # Prepend a decimal point and convert to float
        floats.append(float(f"0.{str_chunk}"))
    return floats
"""

    floats = []
    for i in range(0, len(nums), 7):
        chunk = nums[i:i+7]
        # Format each number to 2 digits
        joined = ''.join(f"{n:02d}" for n in chunk)
        # Put a decimal after the first two digits
        float_str = f"{joined[:2]}.{joined[2:]}" if len(joined) > 2 else joined
        floats.append(float(float_str))
    return floats


print("Processing picture...")
indexed_list = process_image_to_index_list("input.png")
print("Saving picture as list...")
save_as_tireallist(indexed_list, var_name="PIXEL", filename="output")
