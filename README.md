Movie Torrents
==============

说明
---------------

一直使用 zogvm 2.0 <https://github.com/zogvm/zogvm> 管理电影种子文件，扫描整个文件夹时间比较长，豆瓣关闭公共API后无法查询。


特点
--------

- 监视文件夹，即刻处理新添加文件
- 使用豆瓣查询页面内API，如果遭限制，可以IE登录，程序读取IE cookie提交访问


运行
---------

- VS2017 C#编写，自行编译后置于zogvm 目录下
- 使用 zogvm 数据库，添加 seecomment字段到表tb_file 和视图 filelist_view

限制
-------

- 只管理一个目录，tb_dir 第一条记录
- 自用工具，简单第一

