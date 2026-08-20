<p align="center">
  <kbd>    
    <img alt="Lamina Promo" src="https://github.com/user-attachments/assets/09a1d838-7571-4929-82a4-19c88c12889e" />
  </kbd>
</p>

<div align="center">
  
Lamina ✦ is a `WinUI 3 calculator` that is not only includes a Regular Calculator but also something called `"Scripties"`. She supports **Mensuration, Finance, Currency Conversion, Unit Conversions And More!**, making her a Very Extendable Option.

To get User-Made Modules, go to [Lamina ✦ Modules Repo](https://github.com/Chill-Astro/Lamina-Modules-Repo).
  
**Target OS:** **Windows 11** ONLY.  |  **Latest Stable Version:** **v11.28000.16.0**

**App Execution Aliases :** `lamina.exe` & `lmna.exe` 

**Codename** : `LMNA`, formerly `Calc+ ( till v11 )` & `CalcX-11 ( Pre-Alpha )`

*- TRUSTED SOURCES -*

<a href="https://github.com/Chill-Astro/Lamina-Calculator/releases/latest" target="_blank"><img src="https://img.shields.io/static/v1?label=%20&message=GitHub&color=FFFFFF&labelColor=000000&style=for-the-badge&logo=github&logoColor=FFFFFF" height="80" alt="GitHub"></a> 
<a href="https://sourceforge.net/projects/lamina-calculator/" target="_blank"><img src="https://img.shields.io/static/v1?label=%20&message=SourceForge&color=EE7034&labelColor=000000&style=for-the-badge&logo=sourceforge&logoColor=EE7034" height="80" alt="SourceForge"></a> 

<a href='https://lamina.en.uptodown.com/windows' title='Download Lamina - A FOSS and Memorible Calculator!' ><img src='https://stc.utdstc.com/img/mediakit/download-gio-big.png' alt='Download Lamina - A FOSS and Memorible Calculator!'></a>

</div>

> [!NOTE]
> Project [Trust My Msix!](https://github.com/Chill-Astro/Trust-My-Msix) can be used to Import the Self-Signed Certificate of the `.msix ` Version of Lamina ✦. A Copy is given in [Releases](https://github.com/Chill-Astro/Lamina/releases/latest) for Usage.

---

## What are Scripties?

Scripties are High Performance GUI equivalents of Console Scripts, that are Reliable and Easy to Make. 

Categories :

- Built-In Scripties - Made in XAML & C# and Compiled. ( Static )
- User Made Scripties - Made in JSON and loaded by my Interpereter `Dynamo`. ( Dynamic )

---

## A Temporary Halt on Development :

- I wouldn't provide updates for the next couple of months.
- Issues will be addressed  and Pull Requests may be merged a bit late.
- All of this is because WinUI 3 is annoying and in return, no appreciation.
- I am not Maintaining this Project for a Wall, I am maintaining this as an example of What UI can be.
- v11.28000.17.0's Scientific Calc is VERY Confusing to make and will take Time & Experience ( in Math ).
- I will be making some of my own modules.
 
---

## Features :

- Simple and Clean GUI. ✅
- Dozens of calculation options. ✅
- Programmable using JSON-based Logic. ✅
- Fast and Error-Proof Calculations. ✅
- High Precision for decimals. ✅
- Modern UI with Fluid Animations and Transitions. ✅
- History Support for the Base Calculator UI. ✅
- Theme switching built in. ✅
- Backdrop switching betwwen Mica Alt, Mica and Acrylic! ✅
- Eggcelent Looking Splash Screen that can be toggled OFF. ✅
- Available in both Msix & Installer Variants. ✅

---

## Anti-Features :

- Slow Updates ❌
- Use of AI Assistance as I am learning UI and WinUI 3 with AI. ❌
- Lack of Scientific Calculator ( Will be Fixed ) ❌
- Use of `Alex Brush` in Branding which may look like Alien Language for certain People. ❌
- BUGS may Slip in because AI is Stupid, and without a Manual Audit, it's basically `AI SLOP`. ❌
    
---

## Install Lamina ✦ from Winget :
      
      winget install Lamina

---

## Version Structure :

<div align="center">

<H2>

v`11`.`26100`.`16`.`0`

</H2>

</div>

- `11` -> Target OS ( She IS for Windows 11 )
- `28000` -> Release SDK Version ( Currently She uses 28000.xxxx Versions of Windows 11 SDK )
- `16` -> Release Index ( Here 16 stands for the 16th Release Of Course! )
- `0` -> Filler Number ( Package.appxmanifest doesn't allow me to edit this Number so it's there for NOTHING 💀 )

---

## More Screenshots :

<details><summary><b>View Screenshots</b></summary>

<kbd>
<img alt="image" src="https://github.com/user-attachments/assets/93ceb201-e3ff-4bd3-adae-1415efa5b0ae" />
</kbd>

<kbd>
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/dadf0bd6-fefe-41c4-a632-004c7c0542d0" />
</kbd>

</details>

---

## Installation : 

1.  Download the `.msix` and `.cer` files from the [latest release.](https://github.com/Chill-Astro/Lamina/releases/latest)
2.  Import the `.cer` file to the `Trusted Root Certificates` Store. ( First Run ONLY! )

    Alternatively use this Command after Downloading `TMM.exe` from [Releases](https://github.com/Chill-Astro/Lamina/releases/latest) :    

         .\tmm --i <Certificate>
    
    Also you can Paste the Path into the Terminal Prompt or Drag the `.cer` on TMM.exe!
    
4.  Install the `.msix` file.

<p align="center">
  --------------------- OR ---------------------
</p>

-  Download the `Setup.exe` that does the Importing and Installation FOR YOU!

---

## DETAILS ON DYNAMO :

Dynamo Scriptie Loader (DSL) is an `Interpreter` for `JSON` Code for creating a Dynamic UI for User-Made Math. 

Features :

- Dynamically Generates a UI with the Required Fields and Formulae Entered by the User.
- ONE PAGE, MULTIPLE POSSIBILITIES. Yes, `DyNamoPage.xaml` is the Backbone of Any Scriptie Mod.
- Integrated Manager `DyNamoManagerPage.xaml` that Interprets the Mods and feeds this data into `DyNamoPage.xaml` to Load the UI.
- <details> <summary><b>Very Clean & Simple Structure</b></summary>
        
        {
          "Metadata": {
            "Name": "", // This is What your Module Shows Up!
            "Author": "", // Write your Username.
            "Version": "", // This is the Version of your Scriptie.
            "Description": "", // Details
            "Repo": "" // Link ( If FOSS )
          },
          "UI": {
            "Formula": "", // Example W = F.S cos (theta)
            "Inputs": [
              { "Header": "Input Label", "Placeholder": "0.0", "Key": "var_name" } // Add as many as you like.
            ]
          },
          "Logic": {
            "Output": "NCalc math string using [var_name]", // Your Output. Example : "`Work Done = ` + F*S*Cos(Theta)"
            "Error": "Message if math fails" // Your Error.
          }
        }
  
</details> 
      
- Thus, a `Massive Time Saver` and `Low Barrier to Entry` as one can make modules in Notepad.
- Since NCalc is NCalc and not a Compiler, the Risk of Malicious Mods is VERY LOW.

Steps to Use Dynamo Scriptie Loader:

- Go to [Lamina ✦ Modules Repo](https://github.com/Chill-Astro/Lamina-Modules-Repo).
- Download your Desired Mod, a `.lamina` File.
- Go to Menu > Press `+ Add` and Add the File from the Popup!

Contributing :

- Fork [Lamina ✦ Modules Template](https://github.com/Chill-Astro/Lamina-Modules-Template).
- Sample Code is Given. Modify as you Need.
- I will add you to the Lamina ✦ Modules Repo

---

## TRAILER on @chill-astro-sfs :

<div align="center">
  <kbd>
  <a href="https://www.youtube.com/watch?v=AWq2MTEJa0I">
    <img src="https://img.youtube.com/vi/AWq2MTEJa0I/maxresdefault.jpg" alt="Watch the Lamina Trailer"style="border-radius:10px;">
    <br>
    <b><h3>▶ Click to Watch the Official Trailer</h3></b>
  </a>
</kbd>
</div>

---

## Building from Source :

- Install Visual Studio 2026 with **WinUI Application Development** and **.NET Desktop Development** workloads.
  - Windows 11.
  - [XAML Styler](https://marketplace.visualstudio.com/items?itemName=TeamXavalon.XAMLStyler) is recommended for contributing.
  - .NET 10.0 Runtime LTS is must.
  - Get the latest Windows 11 SDK (28000.xxxx)
  - Community Edition is sufficient for contributing and testing. Pro and Enterprise Editions can also be used.
  - Github Copilot and Live Share can be skipped for Storage Saving.
 
<img width="1280" height="720" alt="image" src="https://github.com/user-attachments/assets/1f2705e2-de7d-41d5-8dbd-8138d1812730" />

- Get the Code :
  
      git clone https://github.com/Chill-Astro/Lamina-Calculator.git

- Open [Lamina.sln](/Lamina.sln) in Visual Studio.
- All the Code is Inside [src/](/src)
- Hit Deploy as shown in Screenshot. ( Building is Automatically Done while Deploying )

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/69bff8d4-5724-4854-b558-62484ddb6bea" />

Lamina ✦ is now Deployed and now she shall appear in the Start Menu. Enjoy! :)

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/65cc1ddb-da33-472c-941b-3a3e55c5073e" />

---

## Adding Currency Convertor :

Lamina ✦ uses [ExchangeRate-API](https://app.exchangerate-api.com) for Currency conversion. An API key must be manually added in the region indicated.

- Add `appsettings.json` under Lamina/ : Paste the Code into here.

        {
          "LocalSettingsOptions": {
            "ApplicationDataFolder": "Calculator/ApplicationData",
            "LocalSettingsFile": "LocalSettings.json"
          },
          "CurrencyApiKey": "Enter API Key Here"
        }

- Buid and Run the Application as shown above.

---

## Art 

Since Lamina ✦ is Personified as `"She"` ( Because Computers are She ) and has many Anti-Corporate Design Choices,

so here's her Fictional Human Form! ヾ(^▽^*)))

<details><summary><b>View Art</b></summary>

<kbd>
<img alt="Art" src="https://github.com/user-attachments/assets/724831c5-0497-4a53-8be8-53e6a974412f" />
</kbd>

</details>

Well I am no Artist, but I hope that you Have a Nice Time with Her!

---

## Icon Sources and Credits :

- [Icons8](https://icons8.com) : For all the Mensuration and Quadratic Equation Solver Menu Logos, 
- [SVG REPO](https://www.svgrepo.com/) : For Calculator Menu Logo, Unit Convertor, Heron's Formula, and most of the icons.
- [Icomoon](https://icomoon.io/) : For the Base Calculator Icon and Produce the `.ttf` file for the Icons.
- [Inno Setup by JRSoftware](https://jrsoftware.org/isinfo.php) : Literally the Installer is possible THANKS to them!
- [Microsoft Calculator](https://github.com/microsoft/calculator) : For Square Root and Cube Root Button Icons. Also this inspired me to make this app.
- [ExchangeRate-API](https://app.exchangerate-api.com) : For Currency Conversion. ( Free Plan, so Currency Conversion is Limited! -_- )
- [@Lisa on Pexels](https://www.pexels.com/photo/pink-flowers-photograph-1083822/) : For Wallpaper for Promo Art.

---

## HALL OF FAME 👍 : 

// Will add Forked Repos which are genuinely good. 🤩 I will list everything Good about them.

---

## HALL OF NEUTRALITY 😐 :

// Will add Inactive Forks. Uh yeah that's it atleast it's Forking not Cloning! 😅

- moayyaed/Lamina-Calculator

---

## HALL OF SHAME 👎 :

// Includes Clones who are working against the MIT Licence and Distributing Malware. All Flaws are mentioned. 😑

- She has previously Calculator has undergone Malware Attacks.

---

## ⚠️ IMPORTANT NOTICE ⚠️

Please be aware: There are fraudulent repositories on GitHub that are cloning this project's name and using AI-generated readmes, but they contain **completely random and unrelated files in each release**. These are NOT official versions of this project.

**ALWAYS ensure you are downloading or cloning this project ONLY from its official and legitimate source:**
`https://github.com/Chill-Astro/Lamina-Calculator`

I am trying my best to report these people.

---

## ⚠️ Smoking Gun for Danger :

> [!CAUTION]
> **MALWARE ALERT:** If your downloaded folder looks like the images below, **DO NOT OPEN** any files. Format the drive or delete the folder immediately. Official releases are ONLY `.msix` files or an Inno Setup `.exe`.

<details>
<summary><b>View Details</b></summary>
  
* **Suspicious Windows Executables:** Files ending in `.exe`, `.bat`, or `.dll` (e.g., `luau.exe`, `StartApp.bat`).
* **Compressed Archives:** This project is distributed as an **MSIX**, never as a `.zip` or `.7z` containing Windows binaries.
* **Hidden Scripts:** Text files like `asm.txt` used to execute malicious code on your PC.
* The Following Folder Structure is used by Malware (Shown in a VM) :

![Screenshot_2026-03-01-18-52-39-337_com clone android dual space](https://github.com/user-attachments/assets/be691c9f-7def-4e8b-982c-c7ca2e9a067d)

![Screenshot_2026-03-01-18-53-09-759_com clone android dual space](https://github.com/user-attachments/assets/1c75031d-95be-4716-9347-b762e3dad5b8)

</details>

---


## Note from Developer :

Appreciate my effort? Why not leave a Star ⭐ ! Also if forked, please credit me for my effort and thanks if you do! :)

---
