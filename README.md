# ShadUI ✨

ShadUI (shad·UI /ˈʃæd juː aɪ/) is an **Avalonia-based Desktop UI Library** inspired by [shadcn/ui](https://ui.shadcn.com/)
and [Suki UI Library](https://kikipoulet.github.io/SukiUI/).  
Our goal is to deliver a modern, beautiful, and intuitive UI library for [Avalonia](https://avaloniaui.net/).

[![NuGet](https://img.shields.io/nuget/v/ShadUI.svg)](https://www.nuget.org/packages/ShadUI)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ShadUI)](https://www.nuget.org/packages/ShadUI)
[![GitHub stars](https://img.shields.io/github/stars/accntech/shad-ui)](https://github.com/accntech/shad-ui/stargazers)

**Forever free and open-source.** 🚀

![Hero Image](https://raw.githubusercontent.com/accntech/shad-ui/main/docs/hero.png)

---

## Installation 🚀

To get started with ShadUI:

Install the [ShadUI NuGet package](https://www.nuget.org/packages/ShadUI/):

```powershell
dotnet add package ShadUI
```

> Note: The command above will install the latest version automatically. You can also specify a version if you want to
> install a specific version or install a preview version.

```powershell
dotnet add package ShadUI --version [version]
```

Include ShadTheme in your App.xaml:

```xml
<Application
    xmlns:themes="clr-namespace:ShadUI;assembly=ShadUI">

    <!-- other code -->

    <Application.Styles>
        <themes:ShadTheme />
        <!-- other styles -->
    </Application.Styles>
</Application>
```

> Recommended: Use the `ShadUI.Controls.Window` instead of `Avalonia.Controls.Window` to get the full ShadUI experience.

<a href="https://apps.microsoft.com/detail/9n3358b0phg4?hl=en-us&gl=PH&ocid=pdpshare" target="_blank"><img src="https://learn.microsoft.com/en-us/windows/apps/images/new-badge-dark.png" alt="App Screenshot" width="200"/></a>

[Download](https://apps.microsoft.com/detail/9N3358B0PHG4?hl=en-us&gl=PH&ocid=pdpshare) ShadUI App from Microsoft
Store for examples of how to use ShadUI controls.

## Features 🌟

### 🎨 Theme

ShadUI provides a default theme out of the box, allowing you to get started quickly with a professional look and feel.

![Theme Demo](https://raw.githubusercontent.com/accntech/shad-ui/main/docs/demo-01.gif)

---

### 🛠️ Controls

ShadUI includes a growing set of essential UI controls, inspired by [shadcn/ui](https://ui.shadcn.com/)
and [Suki UI Library](https://kikipoulet.github.io/SukiUI/).  
We are actively working to expand the library with additional controls and advanced features.

![Controls Demo](https://raw.githubusercontent.com/accntech/shad-ui/main/docs/demo-02.gif)

---

### 💬 Dialogs

ShadUI offers a simple yet powerful system for dialogs, enabling you to create interactive and responsive user
experiences effortlessly.

![Dialogs Demo](https://raw.githubusercontent.com/accntech/shad-ui/main/docs/demo-03.gif)

---

### 🔔 Toasts

Deliver quick, non-intrusive messages with ShadUI's built-in toast notifications system, designed for clarity and ease
of use.

![Toasts Demo](https://raw.githubusercontent.com/accntech/shad-ui/main/docs/demo-04.gif)

---

### 🧩 Composable Sidebar

ShadUI provides a flexible and composable sidebar system that allows you to create dynamic navigation experiences with ease.

![Sidebar Demo](https://raw.githubusercontent.com/accntech/shad-ui/main/docs/demo-05.gif)

---


## Acknowledgments 💖

ShadUI wouldn't be possible without the inspiration and contributions of the following projects and libraries:

- [Avalonia](https://avaloniaui.net/): A cross-platform XAML-based UI framework with a robust styling system.
- [shadcn/ui](https://ui.shadcn.com/) and [Suki UI Library](https://kikipoulet.github.io/SukiUI/): The foundation of our
  design principles and control ideas.
- [Lucide Icons](https://lucide.dev/): A beautifully curated collection of icons that provides consistent, scalable vector icons for modern applications.
- [Responsive.Avalonia](https://github.com/russkyc/responsive-avalonia): A library for building responsive layouts.
- [LiveCharts](https://livecharts.dev/): A library for creating visually stunning charts.
- [MvvmToolkit](https://github.com/CommunityToolkit): A framework for implementing the MVVM pattern efficiently.
- [Jab](https://github.com/pakrym/jab): A fast compile-time dependency injection container without runtime dependencies.
- [AvaloniaEdit.TextMate](https://github.com/AvaloniaUI/AvaloniaEdit/): A text editor component with syntax highlighting support for Avalonia applications.


Thank you to all the amazing contributors who have helped make ShadUI what it is today!

<a href="https://github.com/accntech/shad-ui/graphs/contributors">
  <img width="150" src="https://contrib.rocks/image?repo=accntech/shad-ui" />
</a>

---

## Special Thanks 🙏

We extend our heartfelt gratitude to these amazing platforms and tools that make ShadUI possible:

<div align="center">
  <img src="https://github.com/dotnet/brand/blob/main/logo/dotnet-logo.png?raw=true" alt=".NET" height="48"/>
  &nbsp;&nbsp;&nbsp;&nbsp;
  <img height="48px" src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.png" alt="JetBrains">
  &nbsp;&nbsp;&nbsp;&nbsp;
  <img height="48px" src="https://avatars.githubusercontent.com/u/139895814?s=48&v=4" alt="shadcn/ui logo">
</div>

- **.NET**: For providing the powerful C# language and cross-platform runtime that powers our applications
- **JetBrains**: For providing free use of <a href="https://www.jetbrains.com/rider">Rider</a>, the IDE used in the development
- **shadcn/ui**: For the beautiful design system and components that inspired ShadUI's visual foundation

## Contributing 🤝

We welcome contributions to ShadUI! Before getting started, please review
our [Contributing Guidelines](https://github.com/accntech/shad-ui/blob/main/CONTRIBUTING.md)
and [Code of Conduct](https://github.com/accntech/shad-ui/blob/main/CODE_OF_CONDUCT.md).  
Feel free to open issues, suggest new features, or submit pull requests.

---

## License 📜

ShadUI is licensed under the **MIT License**. See the [LICENSE](https://github.com/accntech/shad-ui/blob/main/LICENSE)
file for details.
