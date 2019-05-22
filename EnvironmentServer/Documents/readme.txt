简单介绍：collectd/telegraf(收集数据) -------> influxdb(保存数据) -------> grafana(显示数据)

一、下载安装包

telegraf-1.5.0_windows_amd64.zip
wget https://dl.influxdata.com/telegraf/releases/telegraf-1.5.0_windows_amd64.zip
wget https://dl.influxdata.com/influxdb/releases/influxdb-1.4.2_windows_amd64.zip

grafana-4.6.3.windows-x64.zip
wget https://s3-us-west-2.amazonaws.com/grafana-releases/release/grafana-4.6.3.windows-x64.zip 

分别解压下载下来的文件到指定目录，比如：F:/Grafana

 

二、配置

      grafana需要从influxdb中获取数据, 而influxdb中的数据又需要从其他地方收集过来, 常用的收集工具是collectd或telegraf, 而 influxdb 自身集成 telegraf插件, 所以不需要进行专门的配置。

       注意：正斜杠“/”与反斜杠“\”的区别！

1:、在不同系统的文件浏览器：windows是 \ ,linux和unix是 / ，但在windows中没有本质区别。但是由于 \ 也是转义字符的起始字符 ，路径中的 \ 通常需要使用 \ ，但如果是 / 就不需要使用转义 所以通常在路径中使用 /。 2、浏览器地址栏网址（网络路径）是 /。 <img src=".\Image/Control/ding.jpg" /><img src="./Image\Control\cai.jpg" />本地文件路径，/ 和 \ 是等效的<img src="http://hiphotos.baidu.com/yuhua522/pic/item/01a949c67e1023549c163df2.jpg" /> 网络文件路径，一定要使用 / 在编程中常用于表示反斜杠\不是普通的字符，而是路径的分隔符。如用一个字符串存储保存文件的路径时路径为F:\caffe\Temp\image.jpg;则用字符串存储时，应该写为str=“F:\\caffe\\Temp\\image.jpg”; 

1、修改telegraf.conf文件，设置日志文件目录

Specify the log file name. The empty string means to log to stdout.

logfile = "F:/Grafana/server/telegraf/telegraf.log"   ##你修改为自己定义的目录路径，其他的配置不要乱动。

2、修改influxdb.conf，打开HTTP，修改数据保存的路径，也就是数据库文件

Where the metadata/raft database is stored

dir = "F:/Grafana/server/influxdb/meta"   ##修改为你自己的目录路径#meta控制InfluxDB的Metastore的参数，该参数存储有关用户，数据库，保留策略，分片和连续查询的信息

     

3、修改influxdb.conf，打开HTTP，修改数据保存的路径，也就是数据库文件

The directory where the TSM storage engine stores TSM files.   

    dir = "F:/Grafana/server/influxdb/data"   ##修改为你自己的目录路径    #控制InfluxDB的实际分片数据的存在位置以及如何从WAL刷新数据

 

 4、修改influxdb.conf，打开HTTP，修改数据保存的路径，也就是数据库文件

The directory where the TSM storage engine stores WAL files.

    wal-dir = "F:/Grafana/server/influxdb/wal"    ##修改为你自己的目录路径     #存储WAL刷新数据

5、修改influxdb.conf，打开HTTP，修改数据保存的路径，也就是数据库文件

Determines whether HTTP endpoint is enabled.

  enabled = true  ## 开启，主要作用是接收telegraf的数据并存储，提供API给Grafana调用数据    ### The bind address used by the HTTP service.    bind-address = ":8086"   ## HTTP API使用的端口

6、Grafana使用默认配置。

HTTP端口默认：3000HTTP地址默认：localhost(127.0.0.1)默认用户：admin默认用户密码：admin 数据库类型：sqlite3   ##支持mysql，postgres等数据库地址：localhost(127.0.0.1)数据库端口: 3306数据库名：grafana   ##如果在Windows下连接数据库请在grafana/data下找到grafana.db的文件，用Navicat连接数据库登入用户：root数据库登入密码：root

 

三、启动

4.1 Influxdb 通过cmd命令窗口，切换到influxdb安装目录，执行如下命令：  influxd -config influxdb.conf 

4.2 Telegraf 通过cmd命令窗口，切换到Telegraf安装目录，执行如下命令：  telegraf -config telegraf.conf 

4.3 Grafana 切换到Grafana安装目录中的bin目录下，双击grafana-server.exe启动程序
