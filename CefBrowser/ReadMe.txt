1. 从 https://www.wiz.cn/m/api/WizKMCore-idl-html 下载接口idl
2. 使用 MIDL 从 idl 文件创建 .tlb，幸好这个idl中包含 library 的定义（https://stackoverflow.com/questions/1307675/convert-interface-idl-file-to-c-sharp）
3. 使用 tlbimp.exe 将.tlb 生成程序集 dll
4. 引入这个程序集默认 Embed Interop Types = true，无法设置为 Copy to local = true，需要修改 （https://stackoverflow.com/questions/15526491/why-is-the-copy-local-property-for-my-reference-disabled）
