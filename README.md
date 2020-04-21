# uDimmer
The objective of this project is to be able to save energy and extend the useful life of resources. The idea arose when thinking that due to COVID-19 situation, a lot of equipment for daily use is consuming energy unnecessarily. 
In these cases, an active directory policy is not enough, since it is not automated and only allows it to be activated at specific times. 
This application consumes an a "Flights API" as an example and when detecting that there is a flight in the next three hours activates the high-performance energy plan, in the event that there are no flights, activates the economizer plan.

## Automatize
The purpose of the project is to automate energy saving at a specific time or through an API flag

![alt text](https://raw.githubusercontent.com/unarix/uDimmer/master/udimmer.png)

## How it works

### Powercfg
powercfg (executable name powercfg.exe) is a command-line utility that is used from an elevated Windows Command Prompt to control all configurable power system settings, including hardware-specific configurations that are not configurable through the Control Panel.

uDimmer uses this command with the video and brightness guid to switch from SHEME_MIN to SCHEME_MAX mode. Ejemplo:

** powercfg -setdcvalueindex 079aa309-89cf-4675-88fc-67e4428a4cf1 SUB_VIDEO aded5e82-b909-4619-9949-f5d71dac0bcb 0 **
This enables the system to use brightness at 0 percent.

### Gama Correction (gdi32.dll)
GDI32 exports Graphics Device Interface (GDI) functions that perform primitive drawing functions for output to video displays and printers. in uDimmer is uset to set the gamma correction and alows to set the screen more darkness.

## Features
- scheme of minimum use of energy
- scheme of maximum use of energy
- automate by using apis
- generate reports of energy saving recommendations
- range correction setting