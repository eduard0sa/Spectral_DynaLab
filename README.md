# Spectral DynaLab

<br/>

## 🔊 A digital audio synthesizer software from and for enthusiasts!

Spectral DynaLab was born as a side-project initiated by me (<a href="">eduard0sa</a>), from pure curiosity in unravelling the tech and math frameworks behind major audio production software, as well as one of a series of other projects implementing programming languages like C/C++ and C#.

As a project born from a research environment, this software provides a wide set of tools enabling audio production and mixing with scientific-grade detail, featuring a complete and intuitive Graphical User Interface (GUI) that provides full control over every single parameter.

This application also aims to be a tech showcase, proving how programming can be allied to other completely different fields, like audio production, bringing new ideas and solutions to streamline tasks that otherwise would take way more effort to execute.

<br/>
<div align="center">

![Visual Studio](https://img.shields.io/badge/Visual%20Studio-2022-blue)
![Language](https://img.shields.io/badge/Language-C%2B%2B%20|%20C%23%20|%20C-informational)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)

</div>
<br/>

---

## 🎯Features Overview

**Spectral_DynaLab** is a powerful desktop application designed for audio enthusiasts and researchers who need a flexible and user-friendly audio production suite, packed with advanced tooling specifically crafted to provide full control over a wide range of sound processing parameters.

### Key Features

- **Real-Time Waveform Visualizer**: Visualize audio waveforms with an intuitive wave graph screen.
- **Professional Audio Processing**: Powered by industry-standard JUCE C++ and Rubberband audio libraries (external library reference and copyright notice fully explored on "LICENSE_NOTICE.md").
- **Pitch-Shifting / Time-Stretching**: Modify external audio (.mp3, .ogg, etc...) pitch and/or playback speed with ease.
- **MIDI Track Composition**: Compose melodies and rhythms with a Piano Roll-like UI.

<br/>

---

## 🏗️ Architecture Overview

**Spectral DynaLab includes a variety of interconnected systems with different responsibilities:**

- ***AUDIO_ENGINE:*** C/C++ Audio Processing Engine DLL, powered by JUCE/Rubberband (THE CORE ⚛️).
- ***C/C++ <-> C# COMMUNICATION LAYER:*** Composed by two C++/CLI and C# DLLs that establish communication between the user interface and the ***AUDIO_ENGINE*** layer (THE TRAIN 🚅).
- ***C# .NET MAUI GUI:*** The main application layer, running the Graphical User Interface in which the user interacts and uses all tools and features (THE EDITOR 📊).

#### 📦 Third-Party Libraries
This project uses:
- JUCE (GPL v3) – https://juce.com
- Rubber Band Library (GPL v2) – https://breakfastquay.com/rubberband/
- Font Awesome Free (CC BY 4.0) – https://fontawesome.com

This project is distributed under the GPL v3 license to comply with these dependencies.
