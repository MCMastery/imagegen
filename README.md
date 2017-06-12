# imagegen
Image generation code golf language.

## usage

Download ImageGen.exe from ImageGen/bin/debug, or compile it yourself using VS (make sure ColorCodes.txt is marked as an external resource)

## examples

### blue rectangle

`200x200 RB50,50,75,75`

Generates a 200x200 pixel image with a blue rectangle sized 75x75 at (50,50).

`R` - Filled rect function. Arguments:
- `B` - Blue. See ImageGen/ColorCodes.txt
- `50,50,75,75` - x,y,width,height

### custom color rectangle (using 6-digit hex)

`200x200 #gcdcdcdRg50,50,75,75`

Generates a 200x200 pixel image with a gray (#cdcdcd) rectangle sized 75x75 at (50,50).
`#` - Custom color code declaration. Arguments:
- `g` - The code for this color.
- `cdcdcd` - The hex color 
`R` - Filled rect function.
- `g` - The color code we just created
- `50,50,75,75` - x,y,width,height
