#Usage sample of Quartz.NET

**to change scheduler settings call:**
```http
 POST /schedule/update HTTP/1.1
 Host: localhost:1127
 Content-Type: application/json
 Cache-Control: no-cache
 
 {
 	"isEnabled": true,
 	"IntervalInSeconds": 60,
 	"startDate":1500113744472
 }
```