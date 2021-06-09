/*
--- TODO ---
- Improve narrative with letters and other human artifacts
- Beautify island and add WAY more collectables
- Add saving/loading
- Fix the launch/landing of the character controller
- Add the glide animation into the flight loop
    (could do now, but I want to add acceleration-aware gliding)
- Add wildlife (bees, smaller birds, butterflies, turtles)
    (I have the models and animation I just need to put them in)
- Smooth out the FPS camera, disallow in flight
- Fix the paused walking animation
- Add flap sounds to the landing animation
    (takes a lot of trial and error because I cannot figure
    out how to preview the animation)


From Ricky:
1. Disable the W while taking off for a couple seconds. Maybe for A and D as well. S is fine <- solved by takeoff animation

2. Make "FPS Controls" stand out more. Call it rather "First Person Controls". 
3. Make the controls and actions present more defined. Separate them more. Maybe put a line down between the control and the action
4. Add DPI tuner.
5. Make the help menu close when I press play if its still open. 
6. Make the Help menu icon smaller. 
20. "Aerie" in the pause menu seems too light
21. You can see Beta behind the play button
23. I didnt notice, but i dont think there was an option to rotate the item while holding right click.
But for some reason i thought there was. To rotate hold or toggle rotate with R while holding right click,
then use the mouse to rotate in all directions, or use WASD or Arrows for fine tuning. left click to place <- todo
^^ <- redoing all menus

7. Make the first person cross hair smaller. You could try black instead of red. Or maybe an ocean blue, or a color more fit for the game
<- crosshair is going to have a hole in the middle too

9. Add a true or false toggle feature to  land. Some people may not like it.
They might start landing when they dont want to. Kinda liautoke in Assassin creed when you climb a wall out of nowhere
<- maybe

9.5. For the auto land. Maybe instead make it so i can only fly only so close the ground. Then when i want to land i press Space. 
Right now i can nose dive into the ground. Make it so if i start nose diving, around 3 meter or so the bird will auto pull up and 
level out around 1 meter. Same thing when about to hit a wall. 3 meters out start to pull up then level out at 1. 
You would have to code so i pull up sooner or later based on speed. Make the bird auto dodge all vertical colliders. 
Like i wouldnt land on the wall of the house, or the trunk of a tree. Imagine like Witcher 3 how the horse auto runs around trees 
<- too much work

10. Make it so i float down with my wings out if i walk off a roof. 
The descending wing animation. Auto land when i come close to the ground after walking 
off a roof. Or also while falling allow me to start flying again(edited)
<- working on now

12. Make it so I cant hold middle mouse. If i hold it more then a second it will 
bring up the first person cross hair. If i get real crazy and hold middle mouse, 
plus hold right click, then left click, i can place an item while in third person.
<- need to do this

13. Keep item limit no more then 10. Or if i pick up the same two items put a multiplier next to it
<- good idea

14. Allow me to hide my inventory with "TAB". Open it up with "TAB". Or if i hold right click while first person it auto opens
<- will auto hide in third person

14.5. Give the inventory a little border around all the items you collected. 
Then another border around each item collected. Just make minecrafts hot bar
<- good idea

16. Either disable first person while flying entirely. Or lock the camera to my head or something. Its funky right now.
<- do need to do this

17. Allow me to press a button to auto land on the nest when I'm near it.
<- will do this

18. Dont allow me to place items outside the nest. Unless i can build a nest
 somewhere else. If i want to build a new nest i have to collect a certain number 
 of items to place the nest foundation. If the player is on the ground, and is first person, 
 and has enough items to make a nest, have a ui pop up saying press this button to place a new nest. 
 Say how many more items they need if otherwise. 7/10 items needed to build new nest
<- too complicated but maybe

22. Theres no quit button(edited)
<- todo

*/