# Orchard Module Profiles Readme



## Project Description

An Orchard module for creating module profiles for different environments (eg. development, release).
Using the admin interface, you can create different sets of features and corresponding states (enabled/disabled) and activate (and inverse-activate) them in the admin interface and the command line tool.


## Read a full description about this module in our blogs:

- magyarul: [Új modul a galériában: OrchardHUN.ModuleProfiles](http://orchardproject.hu/blog/uj-modul-a-galeriaban-orchardhun.moduleprofiles)
- in English: [Introducing OrchardHUN.ModuleProfiles](http://english.orchardproject.hu/blog/introducing-orchardhun.moduleprofiles)


## This module is available in the Orchard Gallery:

[Link to ModuleProfiles @ Orchard Gallery](http://gallery.orchardproject.net/List/Modules/Orchard.Module.OrchardHUN.ModuleProfiles)


## How to use:

- Go to: Dashboard - Modules - Module Profiles (or ~/Modules/ModuleProfiles)
- Enter a profile name and click "Create Profile"
- The checkboxes on the left column define whether the a module is included in the profile, the right ones define the modules' state in the profile
- Save the profile after editing it's definition
- You can "Activate" a module, and if you want to activate a profile with inverse module states, click "Activate Inverse" after selecting a profile from the list
- Using "Save Current Configuration" you can save the current configuration of modules into a profile


## Available commands:

- Listing available profiles:
	- moduleprofiles list
	- modprofs lst
- Activating a profile:
	- moduleprofiles activate <ProfileName\>
	- modprofs act <ProfileName\>
- Inverse-activating a profile:
	- moduleprofiles inverse activate <ProfileName\>
	- modprofs inv <ProfileName\>
- Saving the current configuration:
	- moduleprofiles save <ProfileName\>
	- modprofs save <ProfileName\>
- Deleting a profile:
	- moduleprofiles delete <ProfileName\>
	- modprofs del <ProfileName\>

### See the [Version history](Docs/VersionHistory.md)


The module's source is available in two public source repositories, automatically mirrored in both directions with [Git-hg Mirror](https://githgmirror.com):

- [https://bitbucket.org/Lombiq/orchard-module-profiles](https://bitbucket.org/Lombiq/orchard-module-profiles) (Mercurial repository)
- [https://github.com/Lombiq/Orchard-Module-Profiles](https://github.com/Lombiq/Orchard-Module-Profiles) (Git repository)

Bug reports, feature requests and comments are warmly welcome, **please do so via GitHub**.
Feel free to send pull requests too, no matter which source repository you choose for this purpose.

This project is developed by [Lombiq Technologies Ltd](http://lombiq.com/). Commercial-grade support is available through Lombiq.