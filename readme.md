# Circle Lane

Circle lane is a shoot em up game similar to Azur Lane's gameplay and Touhou. The purpose of this project is to prototype a gameplay system, but also to try out Unity's new visual scripting tool called Bolt.


## Structure

Inside the game are two Scenes: TestScene1 and TestScene2.
* TestScene1 uses C#, and while the code is a bit untidy, it works smoother.
* TestScene2 uses Bolt exclusively, and mirrors the functionality of TestScene1, but the boss only has one phase and there are no laser attacks.


## Bolt Findings

To summarize the findings:
* Coding on Bolt is novel, and for coders it is very time consuming.
* Bolt functionality as it is used in the game heavily mirrors C# procedures, so in this case it's very unfavorably compared to the C# counterpart.
* It is mouse-heavy, my hand hurts.
* the State Machine has a lot of quirks.  Triggering the transitions prove difficult for me, and strangely enough, the State Machine can enter multiple states like a mad man.
* Null is not the same as GameObject's Null.  You need to use a Null Check unit.
* Runtime errors would point to the error node.
* You can have multiple state machines in an object.  This is not advisable, as a State machine can already have a nested state machine inside it.
* You can have a flow machine and a state machine in an object.
* You'll have a hard time sending UnityEvents to specific Bolt macros.

In conclusion, Bolt might be good for those learning Unity, but might be a detriment for those already familiar with C#.