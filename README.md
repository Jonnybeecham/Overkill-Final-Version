# Overkill-Final-Version
  This is a game that was created in eight weeks by a team of four students. The game is an RPG battle system. I made the StatsScript
myself and helped in the writing of some of the other scripts. The StatsScript is one of the more important scripts in that it holds all
of the information of the attack, defense, magic, and so forth. It is called by other scripts to store this information for both 
of the main characters and enemies. Some of the challenges I faced while writing the StatsScript were having to assign the values to
the second playable character and making critical hits. The problem with the second character was that he
was a clone of the main charater. This made it a bit tricky to give damage values to his attacks and other stats. So, I talked to my
team, and we found that if we added an if statment to ask if it was the clone, then we could assign the attack damages and
what ever else we need assigned. The problems with the making criticals hits was more of a mathematical problem. I ended up having
to make a second statment for the critical hits altogether. After doing this, it proved to be a good idea that made it easier to 
do critical hits for all types of damage. 
  The PlayerScript holds all of the attacks for the players. It is also were you will find the players attack and defense animations.
The MainBattleMenuScript is the script the has the main menus of the battle such as the battle buttons, the battle items buttons, and
the escape button. The HealthScript holds the health for all of the players and enemies in the game and is refered to when damage
is taken.The EnemyScript and EnemyAIScript may seem similar; however, the EnemyScript stores the basic information of the enemies
such as checking to see if the enemy is dead and telling the enemy how to move toward and away from the player; the EnemyAIScript
is like the brain of the enemy in that it tells the enemy to attack, to heal, or to heal the other enemy. The CursorScript is an
important script that is used to creat a cursor for the game that allows the player to navigate through the menus as well as choose
attack and heal targets. The CameraScript is a nice script that allowed us to make the camera do some extraordinary camera movements
during different attacks such as zooming out and to the side for magic attacks or zooming in for some melee attacks. The BattleSkillSelectPanelScript
is the main script used throughout the battle. It looks simple, but it also is what makes the battle possible. It allows the player to 
go from each menu as well as letting the game know what attack or menu the player has chosen.
 