# Index
- [DailyDuty](#dailyduty)
    + [General Terminology](#general-terminology)
      - [Login Reminder](#login-reminder)
      - [Zone Change Reminder](#zone-change-reminder)
  * [Currently Implemented - Daily Tasks](#currently-implemented---daily-tasks)
    + [Treasure Maps](#treasure-maps)
    + [Mini Cactpot](#mini-cactpot)
  * [Currently Implemented - Weekly Tasks](#currently-implemented---weekly-tasks)
    + [Custom Deliveries](#custom-deliveries)
    + [Wondrous Tails](#wondrous-tails)
      - [Duty Start Notification](#duty-start-notification)
      - [Duty End Notification](#duty-end-notification)
      - [Reroll Alert](#reroll-alert)


# DailyDuty
DailyDuty is a XivLauncher/Dalamud plugin.

DailyDuty allows you to easily and automatically track various daily and weekly tasks.
DailyDuty is designed for people that need _**constant reminders**_ to do things or else they will entirely forget to do them.

![image](https://user-images.githubusercontent.com/9083275/151653259-b369c0d7-cb86-474c-8ef3-f5146db6f123.png)

### General Terminology

**All chat messages can only been seen by you, Dalamud Plugins are client side only**

#### Login Reminder 
A chat message that is displayed 3 seconds after logging in. 

You will only receieve messages for modules that are both `Enabled`, and have `Login Reminder` checked. 

A reminder will not be displayed if there is nothing for you the user to do relating to that module.

#### Zone Change Reminder
A chat message that is displayed each time you change zones.

Most modules will not send a reminder when you enter a duty-bound instance.

This is intended to be the primary way the plugin *pesters you to do things*.

## Currently Implemented - Daily Tasks

### Treasure Maps
Automatically tracks the time and date when you gather a treasure map and will alert you if 18 hours have passed since then.

![image](https://user-images.githubusercontent.com/9083275/151653466-5a7ead81-09db-4804-ba6d-40c361700aec.png)

Map Acquisition Notification - Displays in chat the name of the map, and the recorded time a map was gathered. This is for peace of mind that the plugin captured the data correctly.

Harvestable Map Notification - If you are able to harvest a treasure map when loading into a zone, you will be altered to the names of the maps that can be gathered in the zone you are moving into.

Minimum Map Level - This allows you to set the minimum level of map you want to receive an Harvestable Map Notification for. The level is the "Recommended Player Level" as reported by the map's tooltip.

### Mini Cactpot
Automatically tracks how many Mini Cactpot tickets you have purchased each day.

![image](https://user-images.githubusercontent.com/9083275/151653526-393c7a48-1329-4e8e-b255-f6d15e7bc4d3.png)

## Currently Implemented - Weekly Tasks

### Custom Deliveries
Automatically tracks how many weekly Custom Delivery turn-ins you have used.

![image](https://user-images.githubusercontent.com/9083275/151653551-6dd348d8-bc26-4c7a-9f9c-7ef7c526adbf.png)

### Wondrous Tails
This one is really neat! A must-have for sure!

Automatically tracks your Wondrous Tails book.

![image](https://user-images.githubusercontent.com/9083275/151653603-16af1007-eefb-48ec-8a32-a24212a388f8.png)

#### Duty Start Notification
When entering a duty, if that duty is in your wondrous tails book you will get one of the following messages in chat depending on the state of that task in your book.

![image](https://user-images.githubusercontent.com/9083275/151653729-f4023f67-a165-43c0-9646-ac2b3d6d2fc3.png)

This lets you be efficient with your use of Second-Chance points. As the notification is given at the start of a duty, you can claim, re-roll, and complete the duty for a sticker without needing to gamble which task will be shuffled by rerolling!

#### Duty End Notification
When completing a duty that you can claim for a sticker, once you leave the duty, a reminder message will be sent to remind you to collect the stamp.

#### Reroll Alert
When changing zones, if reroll alert is on, and you are at 9 Second-Chance points, a message suggesting you should shuffle your stickers will be displayed. This is for those that want the best chance at getting 3-lines!

_Recommended companion plugin: ezWondrousTails by daemitus._
