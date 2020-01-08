# proton-cs-min

This is **minimal program for getting data from the Proton RFID reader using C#**. It was written with the intention of being run on a **Linux** machine, using the **Mono** compiler and runtime environment.

## Getting Started

### Installation and Compilation

1. Download this repository onto your machine.
1. Open up a terminal.
1. Navigate, in the terminal, to this repository's directory.
1. Run `mcs -r:CAENRFIDLibrary.dll ProgramEthernet.cs`.
1. Run `mcs -r:CAENRFIDLibrary.dll ProgramUSB.cs`.

### Configuring Ethernet Connection

#### On a Latptop

1. Turn off wifi.
2. Go into Settings -> Network -> Wired and make sure that your computer's address is set to 192.168.1.50.
3. Unplug the Proton from your computer, plug it back in, and reset the Proton.

You won't be able to use wifi again until you've finished with the Proton.

## Run

1. Run either `mono ProgramEthernet.exe` or `mono ProgramUSB.exe`, as required.
