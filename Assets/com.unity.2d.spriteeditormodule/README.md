# UPM Package Starter Kit

The purpose of this package template starter kit is to provide the data structure and develpment guidelines for new packages meant for the **Unity Package Manager (UPM)**.

This is the first of many steps towards an automated package publishing experience within Unity. This package template starter kit is merely a fraction of the creation, edition, validation, and publishing tools that we will end up with.

We hope you enjoy your experience. You can use **#devs-packman** on Slack to provide feedback or ask questions regarding your package development efforts.

## Are you ready to become a package?
The Package Manager is a work-in-progress for Unity and, in that sense, there are a few criteria that must be met for your package to be considered on the package list at this time:
- **Your code accesses public Unity C# apis only.**  If you have a native code component, it will need to ship with an official editor release.  Internal API access might eventually be possible for Unity made packages, but not at this time.
- **Your code doesn't require security, obfuscation, or conditional access control.**  Anyone should be able to download your package and access the source code.
- **You have no urgent need to release your package.**  Our current target for new packages is aligned with 2018.1. Although, based on upcoming package requests and limited packman capacity, that release date is not assured for any package.
- **You are willing to bleed with us a little!** Packman is still in development, and therefore has a few rough edges that will require patience and workarounds.

## Package structure

```none
<root>
  ├── package.json
  ├── README.md
  ├── CHANGELOG.md
  ├── LICENSE.md
  ├── QAReport.md
  ├── Editor
  │   ├── com.unity.[your-package-name].Editor.asmdef
  │   └── EditorExample.cs
  ├── Runtime
  │   ├── com.unity.[your-package-name].Runtime.asmdef
  │   └── RuntimeExample.cs
  ├── Tests
  │   ├── Editor
  │   │   ├── com.unity.[your-package-name].EditorTests.asmdef
  │   │   └── EditorExampleTest.cs
  │   └── Runtime
  │       ├── com.unity.[your-package-name].RuntimeTests.asmdef
  │       └── RuntimeExampleTest.cs
  ├── Samples
  │   └── SampleExample.cs
  └── Documentation
      ├── your-package-name.md
      └── Images
```

## Develop your package
Package development works best within the Unity Editor.  Here's how to set that up:

1. Fork the `upm-package-template` repository

    Forking a repository is a simple two-step process. On GitHub, navigate to the [UnityTech/upm-package-template](https://github.com/UnityTech/upm-package-template) repository.
    Click on the **Fork** button at the top-right corner of the page, and follow the instructions.
    That's it! You now have your own copy (fork) of the original `UnityTech/upm-package-template` repository you can use to develop your package.

    Naming convention for your repository: `upm-package-[your package name]`
    (Example: `upm-package-terrain-builder`)

1. Start **Unity**, create a local empty project.

    Naming convention proposed for your project: `upm-package-[your package name]-project`
    (Example: `upm-package-terrain-builder-project`)

1. In a console (or terminal) application, go to the newly created project folder, then clone your newly forked repository into the packages directory.
    ```none
    cd <YourProjectPath>/UnityPackageManager
    git clone git@github.com:UnityTech/upm-package-[your package name].git com.unity.[your package name]
    ```
    __Note:__ Your directory name must be the name of your package (Example: `"com.unity.terrain-builder"`)
1. Fill in your package information in file **package.json**

    * Required fields:
        * `"name"`: Package name, it should follow this naming convention: `"com.unity.[your package name]"`
        (Example: `"com.unity.terrain-builder"`)
        * `"version"`: Package version `"X.Y.Z"`, your project **must** adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

            Follow this guideline:

            * To introduce a new feature or bug fix, increment the minor version (X.**Y**.Z)
            * To introduce a breaking API change, increment the major version (**X**.Y.Z)
            * The patch version (X.Y.**Z**), is reserved for sustainable engineering use only.

        * `"unity"`: Unity Version your package is compatible with. (Example: `"2018.1"`)

        * `"description"`: Description of your package. This appears in the Package Manager UI.

    * Optional fields:

        * `"dependencies"`: List of packages this package depends on.  All dependencies will also be downloaded and loaded in a project with your package.  Here's an example:
        ```
        dependencies: {
          "com.unity.ads": "1.0.0"
          "com.unity.analytics": "2.0.0"
        }
        ```

        * `"keywords"`: List of words that will be indexed by the package manager search engine to facilitate discovery.

        * `"category"`: List of Unity defined categories used for browsing and filtering (**In Development**)

1. Restart Unity. For more information on embedded package see [here](https://confluence.hq.unity3d.com/display/PAK/How+to+embed+a+package+in+your+project).

1. Enable package support in the editor (*Internal Feature*).  From the **Project** window's right hang menu, enable `DEVELOPER`->`Show Packages in Project Window` (*only available in developer builds*).  You should now see your package in the Project Window, along with all other available packages for your project.

1. Update **README.md**

    The README.md file should contain all pertinent information for developers using your package, such as:

    * Prerequistes
    * External tools or development libraries
    * Required installed Software
    * Command line examples to build, test, and run your package.

1. Rename and update **your-package-name.md** documentation file.

    Use this template to create preliminary, high-level documentation. This document is meant to introduce users to the features and sample files included in your package.

1. Rename and update assembly definition files.

    * If your package requires to isolate Editor code (to make sure it's not included in Runtime build), modify [Editor/com.unity.your-package-name.Editor.asmdef](Editor/com.unity.your-package-name.Editor.asmdef). Otherwise, delete the file.
      - Name **must** matches your package name, suffixed by `.Editor`
      - Assembly **must** references `com.unity.[your-package-name].Runtime` (if you have any Runtime)
      - Platforms **must** include `"Editor"`
    * If your package contains code that needs to be included in Unity runtime builds, modify [Runtime/com.unity.your-package-name.Runtime.asmdef](Runtime/com.unity.your-package-name.Runtime.asmdef). Otherwise, delete the file.
      - Name **must** matches your package name, suffixed by `.Runtime`
    * If your package has Editor code, you **must** have Editor Tests. In that case, modify [Tests/Editor/com.unity.your-package-name.EditorTests.asmdef](Tests/Editor/com.unity.your-package-name.EditorTests.asmdef).
      - Name **must** matches your package name, suffixed by `.EditorTests`
      - Assembly **must** references `com.unity.[your-package-name].Editor` and `com.unity.[your-package-name].Runtime` (if you have any Runtime)
      - Platforms **must** include `"Editor"`
      - Optional Unity references **must** include `"TestAssemblies"` to allow your Editor Tests to show up in the Test Runner/run on Katana when your package is listed in project manifest `testables`
    * If your package has Runtime code, you **must** have Playmode Tests. In that case, modify [Tests/Runtime/com.unity.your-package-name.RuntimeTests.asmdef](Tests/Runtime/com.unity.your-package-name.RuntimeTests.asmdef).
      - Name **must** matches your package name, suffixed by `.RuntimeTests`
      - Assembly **must** references `com.unity.[your-package-name].Runtime`
      - Optional Unity references **must** include `"TestAssemblies"` to allow your Playmode Tests to show up in the Test Runner/run on Katana when your package is listed in project manifest `testables`

1. Document your package.

  * **Document your public APIs**
    - All public APIs need to be documented with XmlDoc.  If you don't need an API to be accessed by clients, mark it as internal instead.
    - API documentation is generated from [XmlDoc tags](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/xml-documentation-comments) included with all public APIs found in the package. See [Editor/EditorExample.cs](https://github.com/UnityTech/upm-package-template/blob/master/Editor/EditorExample.cs) for an example.

  * **Document your features**
    - All packages that expose UI in the editor or runtime features should use the documentation template in [Documentation/your-package-name.md](Documentation/your-package-name.md).

  * **Documentation flow**
    - Documentation needs to be ready when a publish request is sent to Release Management, as they will ask the documentation team to review it.
    - The package will remain in `preview` mode until the final documentation is completed.  Users will have access to the developer-generated documentation only in preview packages.
    - When the documentation is completed, the documentation team will update the package git repo with the updates and they will publish it on the web.
    - The package's development team will then need to submit a new package version with updated docs.

11. Add tests to your package.

  * Editor tests
    * Write all your Editor Tests in `Tests/Editor`
    * If your tests require to have access to internal methods, add an `AssemblyInfo.cs` file to your `Editor` code and use `[assembly: InternalsVisibleTo("com.unity.[your-package-name].EditorTests")]`

  * Playmode Tests
    * Write all your Playmode Tests in `Tests/Runtime`.
    * If your tests require to have access to internal methods, add an `AssemblyInfo.cs` file to your `Runtime` code and use `[assembly: InternalsVisibleTo("com.unity.[your-package-name].RuntimeTests")]`

12. Update **CHANGELOG.md**

    Every new feature or bug fix should have a trace in this file. For more details on the chosen changelog format, see [Keep a Changelog](http://keepachangelog.com/en/1.0.0/).


## Register your package
If you think you are working on a feature that is a good package candidate, please take a minute to fill-in this form: https://docs.google.com/forms/d/e/1FAIpQLSedxgDcIyf1oPyhWegp5FBvMm63MGAopeJhHDT5bU_BkFPNIQ/viewform?usp=sf_link.

Working with the board of dev directors and with product management, we will schedule the entry of the candidates in the ecosystem, based on technical challenges and on our feature roadmap.
Don’t hesitate to reach out and join us on **#devs-packman** on Slack.

## Share your package

If you want to share your project with other developers, the steps are similar to what's presented above. On the other developer's machine:

1. Start **Unity**, create a local empty project.

    Naming convention proposed for your project: `upm-package-[your package name]-project`
    (Example: `upm-package-terrain-builder-project`)

1. Launch console (or terminal) application, go to the newly created project folder, then clone your repository in the packages directory

    ```none
    cd <YourProjectPath>/UnityPackageManager
    git clone git@github.com:UnityTech/upm-package-[your package name].git com.unity.[your package name]
    ```
    __Note:__ Your directory name must be the name of your package (Example: `"com.unity.terrain-builder"`)

## Dry-Run your package with **UPM**

There are a few steps to publishing your package so it can be include as part of the editor's package manifest, and downloaded by the editor.

1. Publishing your changes to the package manager's **staging** repository.

    The staging repository is monitored by QA and release management, and is where package validation will take place before it is accepted in production. To publish to staging, do the follow:
      * Join the **#devs-packman** channel on Slack, and request a staging **USERNAME** and **API_KEY**.
      * [Install NodeJs](https://nodejs.org/en/download/), so you can have access to **npm** (Node Package Manager).
      * If developing on Windows, [install Curl](https://curl.haxx.se/download.html).  Note: Curl already comes with Mac OS.
      * Setup your credentials for **npm** by running the following command line from the root folder of your package.
        ```
        curl -u<USERNAME>@unity:<API_KEY> https://staging-packages.unity.com/auth > .npmrc
        ```
      * You are now ready to publish your package to staging with the following command line, from the root folder of your folder:
      ```none
      npm publish
      ```
2. Test your package locally

    Now that your package is published on the package manager's **staging** repository, you can test your package in the editor by creating a new project, and editing the project's `manifest.json` file to point to your staging package, as such:
      ```
      dependencies: {
        "com.unity.[your package name]": "0.1.0"
      },
      "registry": "https://staging-packages.unity.com"
      ```

## Getting your package published to Production

  Packages are promoted to the **production** repository from **staging**, described above. Certain criteria must be met before submitting a request to promote a package to production.
  [The list of criteria can be found here](https://docs.google.com/document/d/1TSnlSKJ6_h0C-CYO2LvV0fyGxJvH6OxC2-heyN8o-Gw/edit#heading=h.xxfb5jk2jda2)

  1. Once you feel comfortable that your package meets the list of Release Management Criteria, [Submit your package publishing request to Release Management](https://docs.google.com/forms/d/e/1FAIpQLSdSIRO6s6_gM-BxXbDtdzIej-Hhk-3n68xSyC2sM8tp7413mw/viewform).

**Release management will validate your package content, and check that the editor/playmode tests are passed before promoting the package to production.  You will receive a confirmation email once the package is in production.**

**You're not done!**  At this point, your package is available on the cloud, 2 more steps are required to make your package discoverable in the editor:
1. Contact the Package Manager team in #devs-packman to ask them to add your package to the list of discoverable package for the Unity Editor.  All you need to provide is the package name (com.unity.[your-package-name])
1. If your package is meant to ship with a release of the editor (Unity Recommended Packages), follow these steps:
      * Modify the editor manifest ``[root]\External\PackageManager\Editor\Manifest.json`` to include your package in the list of dependencies.

      * Submit one or more Test Project(s) in Ono, so that your new package can be tested in all ABVs moving forward.  The following steps will create a test project that will run in ABVs, load your package into the project, and run all the tests found in your package.  The better your test coverage, the more confident you'll be that your package works with trunk.
           * Create a branch in Ono, based on the latest branch this package must be compatible with (trunk, or release branch)
           * If your package contains **EditorTests**:
             * In ``[root]\Tests\EditorTests``, create a new EditorTest Project (for new packages use **YourPackageName**) or use an existing project (for new versions of existing package).
             * A [skeleton of EditorTest Project can be found here](https://oc.unity3d.com/index.php/s/Cldvuy6NpxqYy8y).
             * Modify the project’s manifest.json file to include the production version of the package (name@version).
             * Your project's manifest.json file should contain the following line: ``"testables" : [ "com.unity.[your package name]" ]``
           * If your package contains **PlaymodeTests**:
             * In ``[root]\Tests\PlaymodeTests``, create a new PlaymodeTest Project (for new packages use **YourPackageName**) or use an existing project (for new versions of existing package).
             * Modify the project’s manifest.json file to include the staging version of the package (name@version).
             * Your project's manifest.json file should contain the following line:
             ``"testables" : [ "com.unity.[your package name]" ]``.
           * Commit your branch changes to Ono, and run all Windows & Mac Editor/PlayMode tests (not full ABV) in Katana.
      * Once the tests are green on Katana, create your PR, add both `Latest Release Manager` and  `Trunk Merge Queue` as reviewers.
