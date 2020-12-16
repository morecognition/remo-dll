# Introduction

Remo DLL is a repository which contains the necssary tools for communicating with a REMO device using the Windows operating system. This project contains the following:
* A project which generates a dll (morecognition.dll) that can be used to communicate with a REMO device
* A simple windows forms based application named Data_Recorder which can be used to test the DLL and stream real-time data from REMO
## 1 DLL
The aim of this project is to create a windows shared library (otherwise known as a DLL) to communicate with a REMO device. This project has 3 main parts to it:
### 1.1 Gestione Comunicazione e Thread 
---
This part of the project is responsible for the raw communication with the REMO. This part has the classes and methods necessary to generate AT commands necessary to communicate with a REMO and to configure it.
#### 1.1.1 ActionQueueScheduler
This class is responsible for handling threading but is not being used in the project currently.
#### 1.1.2 ATcommand
This class is the core class which contains mehtods and properties that help build different ATcommands for the configruation of a REMO device and which read the responses of these configruation commnads.
#### 1.1.3 ATcommandExecution
This class uses class [ATCommand](#1.1.2-atcommand) to write methods necessary to execute the AT commands.
#### 1.1.4 MyTaskScheduler
This class is used to schedule and queue different tasks in the [Device](#1.2.1-device) class to assist in serial port communication.
### 1.2 Info Dispositivo e Pacchetto
---
This part of the project is responsible for creating an object "Device" which a software counter-part of the remo device.
#### 1.2.1 Device
This class is responsible for using the classes defined in [Gestione Comunicazione e Thread](#1.1gestionecomunicazioneethread) to connect to a REMO device, configure it (out of the 3 different options - RAW, RAW_IMU and RMS) and lastely to parse the data that is received from the REMO device and to store them in the form of a packet (defined in the [Sample](#1.2.2-sample) class).
#### 1.2.2 Sample
This class defines a packet of data received from a REMO device with all its contects (i.e. the data received from the different sensors on REMO device).
### 1.3 Real Time Data
---
This part of the project uses the private classes defined in [Info Dispositivo e Pacchetto](#1.2infodispositivoepacchetto) to expose them as public classes so that an application which uses the morecognition dll can interact with the device and receive packets of data.
#### 1.3.1 IRawData
This is an interface which contains a method triggered every time a packet as been receievd and correctly parsed. This interface can be implemented by any class to receive data from a connected remo device.
#### 1.3.2 RawData
This class defines the structure of a packet of data. This class is similar to the [Sample](#1.2.2-sample) class but is open to application that tries to access it using a the morecognition dll.
#### 1.3.3 RtData
This class uses an object from the [Device](#1.2.1-device) class and defines methods to connect to the REMO device and stream data from it.
---
## 2 
