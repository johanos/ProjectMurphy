# ProjectMurphy

##Reality, Virtually Hackathon - Team HoloCaptions

###Trello
https://trello.com/b/XBGzL8E2/vr-hackathon

###DevPost
http://devpost.com/software/holocaptions

## Tools

Microsoft HoloLens

HoloToolKit-Unity
https://github.com/Microsoft/HoloToolkit-Unity

Unity 5.4.0f3 - HoloLens Technical Preview Edition


#Run Instructions

To run this project, just clone the repo. 
If you're trying to deploy to a HoloLens emulator, it will give a null pointer exception. Sadly, the Emulator doesn't expose access to a camera so just find the FacialRec.cs script component in the FacialRecManager under the managers drop down in Unity and turn it off. This will let the Dictation component work on its own (sadly no positioning based on facial recognition, but if deployed to a HoloLens this should work). 


