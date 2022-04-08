# Marching-Cubes
An implementation of the Marching Cubes algorithm in Unity.  
  
As for the algorithm's good points, it's an extremely simple algorithm to implement, consisting mainly of a pair of giant lookup tables to connect the vertices into triangles. This simplicity also means it's pretty fast. Also, if you want to create a mesh of the same thing, but higher resolution, you simply have to increase the number of sampling points taken in a given space. Lastly, it's also an extremely general algorithm. You can literally use it with **any** function that takes in a coordinate in 3d space and spits out a number and it will create a mesh. This makes it especially good for procedural terrain generation, as with this algorithm you can create terrain with caves and overhangs, as wll as spherical planets and the such. This is opposed to simple heightmap generation, where such features are impossible.

## How the algorithm works
The marching cubes algorithm is an algorithm to create a 3d mesh from a 3d grid of points. The algorithm works as follows:
1. You start out with a grid of points and a "surface value" alos occasionally called the isosurface value
2. At every point, you assign a value using some function, called the density function. What function it is isn't particularly important for this explanation, as long as it doesn't assign the same value to every point
3. What you can do then is consider every point that has a value below the surface value to be inside the mesh and every point that has a value above the surface value to be outside the mesh
4. So, considering a single set of 8 points in a cube shape, what you can do is find all the edges with one point below the surface and one point above the surface, and create a vertex along that edge
      1. Now, some implementations put the vertex halfway between the two points, however this leads to blocky results.
      2. To get the smoother results this implementation generates, you inversely linear interpolate between the two edge point values using the surface values. As a simple example, if the value of point A was 0 and the value of point B was 5, and your surface value was 4, you would put it 80% of the way from point A to point B, since the surface value is 80% of the way from point A's surface value to point B's Surface value
5. Then the way you'd go about linking those vertices into triangles is using a lookup table, using the particular arrangement of 8 below-and-above surface points that make up the cube as a key
6. You can then go through each cube, generating the vertices and triangles of each one, ending up with a completed mesh.

If you don't get the algorithm, here's a small example in 2d:  
![example](https://cdn.discordapp.com/attachments/647518062328938497/893741809526788157/marching_cube_explanation.jpg)  
It's a fairly popular algorithm, so there are other explanations of it online, if this one doesn't suffice.

## How to use the project
On the right side of the unity editor, you should see several objects in the scene hierarchy. Each one represents a different density function to generate meshes from, listed below  One thing to note is that each of the generators marked "terrain generators" uses a bunch of different small tricks to get the desired effects, so if you want a detailed look at how exactly those work, you're just going to have to look at the source code, as I don't want this readme to be bogged down with a bunch of technical details.  
**Pure Noise:** 3d perlin noise  
**Warped Noise:** 3d perlin noise, warped using the technique shown [here](https://www.iquilezles.org/www/articles/warp/warp.htm)  
**Spherical Noise:** 3d perlin noise, but we subtract the distance from the origin, creating a spherical shape  
**"Best" Terrain:** The first of the terrain generation types, and one I think generally produces nice looking terrain, as the name suggests.  
**Expieremental Terrain:** A terrain generator I was using to test various effects. Right now allows for terracing.  
**Overthought Terrain:** Actually uses 2 layers of noise, one 2d layer to create a heightmap, the other is a 3d layer that gets added on top to create overhangs and such. Also allows for terracing. Named as such because after all of that, there was barely any difference from the other 2 terrain generators.  
**Dot Tester:** This one is the odd one out. Doesn't actually generate a mesh, instead displays the values of each point using a given density function. Points with low value are black, points with high value are white.  
  
If you want to generate a mesh with one, click on it, then click on the generate button that should appear on the right. If the mesh doesn't appear, make sure the object is active (the checkbox next to it's name in the inspector is checked). As for how to create your own settings presets, that's in the next section.  
  
These generators also have their own variables. Here's a quick rundown:  
**Auto Update:** Whether to update the mesh every time a variable is changed. Wouldn't recommend if the terrain size is big.  
**Show Min And Max Values:** Whether to print the minimum and maximum values generated to the console. Would generally be used only if **Use Percentage Surface Level** isn't checked in the marching cubes settings
**Use Flat Shading:** Whether or not to use flat or smooth shading on the mesh.  
**Center:** Where the generation should be centered.  
If you press play, you can fly around your creations with the same controls as minecraft creative mode.  
*One thing to note: If, after you finish generating the mesh, your computer is still slow, make sure you arent highlighting the object with the mesh, For some reason with meshes with a lot of triangles that'a a wierdly slow thing to do with unity*

## Settings
For each of the generators above, you'll have to put in 2 seperate settings objects. If you don't do this, you'll get an error message. The first is a MarchingCubesSettings object, which controls things like terrain size and mesh resolution. The second one is a generator-specific settings object, which is how you control things specific to the type of generation you're doing.  
To create a new instance of each of these objects, go to the project menu and **right click>Create>The type of settings object you want to create**. Here's an explanation of each of the variables you'll find in these settings objects and what they control:

### Marching Cubes Settings
**Size:** These 3 numbers control the # of points sampled in each direction
**Grid Size:** How you control the resolution of the mesh. The space between each point in the grid
**Surface Level:** The surface/isosurface level. See explanation of the algorithm above
**Use Percentage Surface Level:** Whether to use the given surface level as a percentage between the min and max values generated or just as a raw #. if checked, surface level will be constrained between 0 and 1.

### Generator Specific Settings
**Seed:** The seed for the random # generator to use  
**Scale:** The scale of the noise used. Zoom in by increasing this  
**Octaves:** How many octaves of layered noise to use  
**Lacunarity:** How much each octave of layered noise zooms in  
**Persistance:** How much each octave of layered noise affects the result
*Note: The Overthought Terrain Generator has copies of all of the above variables. the ones marked terrain are for the 2d heightmap, The others are for the 3d noise that gets overlaid on top*  
**Amplitude:** How much the noise values affect the final result  
**Noise Strength & Height Scale:** Exclusive to the overthought terrain generator, noise strength is a slider where 0 means the generation is controlled exclusively by the 3d noise and 1 means the generation is exclusively controlled by the heightmap. Height scale is just a straight multiplier to the heightmap  
**Floor Height:** What the floor's y value is
**Floor Strength:** How "strong" the floor is. Increase this to reduce the severity and # of indents in the floor, though it should be noted that doing so when **Use Percentage Surface Level** is set to true will cause the mountains to shrink if increased too far.  
**Use Terracing:** Toggles terracing.  
**Terrace Height:** Exactly what it sounds like.  
**Warp Settings:** Look [here](https://www.iquilezles.org/www/articles/warp/warp.htm) for more details. For performance reasons, a maximum of 3 are allowed.

## Materials
This section is just gonna be a quick overview of only my custom shaders that I made for the project. If you want to create new instances of these materials, create a new material,  navigate to the shader field in the inspector, then go to custom.  
**Terrain Shader:** Pretty simple. Shades things based on height. You can define the # of layers (Max 8, you can use a higher number, but nothing will happen. Similarily, if you set it to a number less than 1, everything will just be drawn with the fist color in the inspector), then for each layer except the first, you can define a color, the minimum height at which that color will appear, and then how much it blends with the color below it.  
**Sphere Shader:** The same as the terrain shader, except you define a center (the w variable is useless) and things are shaded based on how far they are from that center.  
**Normal Based Shader:** Similar to the terrain shader, except instead of shading based on height, you instead shade based on the angle between the normal vector and the down vector. So for each layer, you define the min angle (in radians) that must be between the normal vector and the down vector to be shaded that color

## Examples!
![example](https://cdn.discordapp.com/attachments/647518062328938497/893764163455823872/Marching_Cubes_-_SampleScene_-_PC_Mac__Linux_Standalone_-_Unity_2019.4.3f1_Personal___DX11__9_29_202.png)  
You can see the overhangs I was talking about  
  
![example](https://cdn.discordapp.com/attachments/647518062328938497/893764166769336370/Marching_Cubes_-_SampleScene_-_PC_Mac__Linux_Standalone_-_Unity_2019.4.3f1_Personal___DX11__9_29_202.png)  
Fairly normal looking terrain  
  
![example](https://cdn.discordapp.com/attachments/647518062328938497/893764168795168768/Marching_Cubes_-_SampleScene_-_PC_Mac__Linux_Standalone_-_Unity_2019.4.3f1_Personal___DX11__9_29_202.png)  
A spherical world I made  
  
![example](https://cdn.discordapp.com/attachments/647518062328938497/893764169063612457/Marching_Cubes_-_SampleScene_-_PC_Mac__Linux_Standalone_-_Unity_2019.4.3f1_Personal___DX11__9_29_202.png)  
A zoom in on part of that planet, where you can see a cool formation where a hole was formed  
  
![example](https://cdn.discordapp.com/attachments/647518062328938497/893764171810897930/Marching_Cubes_-_SampleScene_-_PC_Mac__Linux_Standalone_-_Unity_2019.4.3f1_Personal___DX11__9_29_202.png)  
An example of the warped noise generation with the normal based shading  
