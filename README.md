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
`R` - Filled rect function. Arguments:
- `g` - The color code we just created
- `50,50,75,75` - x,y,width,height


### custom color rectangle (using 3-digit hex)

`200x200 @ec00Re50,50,75,75`

Generates a 200x200 pixel image with a red (#cc0000) rectangle sized 75x75 at (50,50).
`@` - Custom color code declaration. Arguments:
- `e` - The code for this color.
- `c00` - The hex color - turns into #cc0000
`R` - Filled rect function. Arguments:
- `e` - The color code we just created
- `50,50,75,75` - x,y,width,height

### grid

`200x200 G04`

Generates a 200x200 pixel image with a black grid with lines spaced 4 pixels apart.

`G` - Grid draw function. Arguments:
- `0` - Line color (black)
- `4` - Spacing between lines
