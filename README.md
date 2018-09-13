# Snai.File
### .net core 读取本地指定目录下的文件  
asp.net core 读取Snai.File.FileOperation/log目录下的.log文件，.log文件的内容如下： 
#### xxx.log 
------------------------------------------begin---------------------------------  
写入时间:2018-09-11 17:01:48  
userid=1000  
golds=10  
-------------------------------------------end--------------------------------- 

一个 begin end 为一组，取同一个.log文件里 userid 相同，写入时间最大一组值，输出结果如下：  
| UserID | Golds | RecordDate |
|:------:|:-----:|:----------:|
| 1001   | 20    |2018/9/11 17:10:48|  
| 1000   | 20    |2018/9/11 17:11:48|  
| 1003   | 30    |2018/9/11 17:12:48|  
| 1002   | 10    |2018/9/11 18:01:48|  
| 1001   | 20    |2018/9/12 17:10:48|  
| 1000   | 30    |2018/9/12 17:12:48|  
| 1002   | 10    |2018/9/12 18:01:48|  

 UserID | Golds | RecordDate 
:------:|:-----:|:----------:
 1001   | 20    |2018/9/11 17:10:48  
 1000   | 20    |2018/9/11 17:11:48  
 1003   | 30    |2018/9/11 17:12:48  
 1002   | 10    |2018/9/11 18:01:48  
 1001   | 20    |2018/9/12 17:10:48  
 1000   | 30    |2018/9/12 17:12:48  
 1002   | 10    |2018/9/12 18:01:48 


