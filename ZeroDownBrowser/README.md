Zero download browser
==============
使用cefsharp的浏览器浏览 0daydown 的内容并自动保存到为知笔记。

2020/9/4 21:45
---------------
从浏览器拷贝内容粘贴，可以看到粘贴的Html代码将元素的 style 展开成 inline 模式，这些style又不是 javascript omputedstyle 计算的所有 style 属性，只是出现在相关 css class 中的。
尝试从 cef 项目源码中找线索： https://bitbucket.org/chromiumembedded/cef/wiki/BranchesAndBuilding 
这个项目本身是基于 google chrome 的，编译的说明一大堆： https://bitbucket.org/chromiumembedded/cef/wiki/MasterBuildQuickStart.md#markdown-header-windows-setup 
代码是托管在google的，下载有问题，git config --global http.proxy http://proxyUsername:proxyPassword@proxy.server.com:port 配置代理后，update_depot_tools 居然没有反应了。
使用 git config --global --unset http.proxy 取消代理

cefsharp https://github.com/cefsharp/CefSharp 当前开发版本可能编译有问题的，需要checkout 一个 release 来编译。
