# blackcat
A bundle of dotnet utilities

![](https://4.bp.blogspot.com/-hN5iDUgWRJE/XcrZ5SqCzHI/AAAAAAAAZb4/FnuR327jKSkWBzXF65Pv542KpEA2uS6KACLcBGAsYHQ/s1600/Untitled%2BDiagram%2B%25281%2529.png)

# Getting started

1. Nuget (comming soon)
2. Dll files (comming soon): [release page](https://github.com/sontx/blackcat/releases)
3. Clone this repo as a submodule and add reference to your .net project

# All important utilities

## Configuration

Saves or loads configurations from file automatically.

### Basic usage

1. Defines your configuration class
```cs
public class MyConfig
{
  public string Config1 {get;set;}
  public int Config2 {get;set;}
}
```

2. Gets configuration and use it
```cs
// MyConfig will be loaded from file or create new one if needed
var myConfig = ConfigLoader.Default.Get<MyConfig>();
textBox1.Text = myConfig.Config1;
```

3. Saves configuration: your configuration will be saved to json file automatically when winform application's closed.
Json file will be something like:
```json
{
  "Metadata": {
    "Modified": "2019-11-11T14:29:35.3268223+07:00"
  },
  "Configs": [
    {
      "Key": "MyObject",
      "Data": {
        "Config1": "This is a string config",
        "Config2": 123,
      }
    }
  ]
}
```

### Advance usage

Saves immediately whenever config property changes

```cs
// Inherits from AutoNotifyPropertyChanged class and marks properties as virtual.
public class MyConfig : AutoNotifyPropertyChanged
{
  public virtual string Config1 {get;set;}
  public virtual int Config2 {get;set;}
}

// Change save mode to:
//  OnChange: save immediately when config changes
//  OnExit: save when app's closed
//  ReadOnly: don't save
ConfigLoader.Default.SaveMode = SaveMode.OnChange;
var myConfig = ConfigLoader.Default.Get<MyConfig>();
myConfig.Config1 = "New config";// config will be saved after this call
```

Edits config at runtime

```cs
[ConfigClass(Description = "Describe your config here")]
public class MyConfig
{
    [Description("First string config")]
    public string Config1 { get; set; }

    [Description("Second int config")]
    public int Config2 { get; set; }
}

var myConfig = ConfigLoader.Default.Get<MyConfig>();
using (var form = new SettingsForm { Settings = myConfig })
{
    form.ShowDialog(this);
    var changed = form.SettingsChanged;
}
```
![Edit config form](https://2.bp.blogspot.com/-LjpeTdWCyto/Xcpu8IG9gTI/AAAAAAAAZbY/LJCu-7O1uDYBM5YSAxqkJ1CIR7jjKiTEgCLcBGAsYHQ/s1600/Capture.PNG)

## EventBus

Lightweight event aggregator/messenger for loosely coupled communication

### Basic usage

1. Defines an event which will carry data from caller to subscriber
```cs
public class MyEvent
{
    public string Data1 {get; set;}
    public int Data2 {get; set;}
}
```

2. Subscribes events
```cs
class MyService
{
    MyService()
    {
        // Start subscribing incomming events (call Unregister to unsubscribe events)
        EventBus.Default.Register(this);
    }

    [Subscribe]
    private void SubscribeMethod1(MyEvent myEvent)
    {
        // do you stuff here
    }

    [Subscribe]
    private void SubscribeMethod2(MyEvent2 myEvent2)
    {
        // do you stuff here
    }
}
```

3. Raises an event
```cs
// Somewhere else in your project

EventBus.Default.Post(new MyEvent {Data1 = "my data1", Data2 = 123});// SubscribeMethod1 will be called
EventBus.Default.Post(new MyEvent1 {...});// SubscribeMethod2 will be called
```

### Advance usage

Subscribes an event in background thread
```cs
// Currently we're supporting several thread modes:
//  Post: default mode, invokes subscribers immediately in current caller thread
//  Thread: invokes subscribers in a new background thread if caller thread is main thread, otherwise invokes subscribers immediately in current caller thread 
//  Async: Always invokes subsribers in a new background thread
//  Main: Invokes subscribers in main thread (UI thread) in blocking mode
//  MainOrder: Invokes subsribers in main thread (UI thread) in non-blocking mode
[Subscribe(ThreadMode = ThreadMode.Thread)]
private void WillBeCalledInBackgroundThread(MyEvent myEvent)
{
    // this stuff will be called in background thread
}
```

Prevents further propagation of the current event
```cs
[Subscribe]
private PostHandled CancelableSubscriber(MyEvent myEvent)
{
    // do you stuff here
    return new PostHandled {Canceled = true};
}
```

Returns data for caller (only supports for **Post** or **Main** ThreadMode)
```cs
// From subscribers
[Subscribe]
private PostHandled ReturnValueForCaller1(MyEvent myEvent)
{
    // do you stuff here
    return new PostHandled {Data = "any data here"};
}
[Subscribe]
private string ReturnValueForCaller2(MyEvent myEvent)
{
    // do you stuff here
    return "any data here";
}

// From caller
var results = EventBus.Default.Post(new MyEvent{...});
```

Keeps the last sticky event of a certain type in memory. Then the sticky event can be delivered to subscribers or queried explicitly.
```cs
// Posts a sticky event
EventBus.Default.Post(new MyEvent{...}, true);// MyEvent likes other events but it's still remaining in memory after called
EventBus.Default.Post(new MyEvent{...}, true);// this will replace the first event

// Queries a specific sticky event
var myStickyEvent = EventBus.Default.GetStickyEvent(typeof(MyEvent));

// Removes a specific sticky event
EventBus.Default.RemoveStickyEvent(myStickyEvent);
```

Communicates with another process uses EventBus

Client process
```cs
var eventbus = ClientEventBus.StartNew("my-eventbus");
eventbus.Post(new MyEvent{...});
```

Main process (ex: a windows service)
```cs
var eventbus = ServerEventBus.StartNew("my-eventbus);
eventbus.Register(this);
.....

[Subscribe]
private void ListenMyEvent(MyEvent myEvent)
{
...
}
```

## AppCrash

Handles and reports unhandled-exception automatically

### Basic usage

```cs
private AppCrash appCrash = new AppCrash(); // defines it as a global variable to prevent GC collects this.
```
If an unhandled-exception occurs, a crash report will be shown with notepad, the log file is also saved in **CrashLogs** folder.
![Crash report](https://2.bp.blogspot.com/-YEUHQsu9YkI/XcqS6PpEJnI/AAAAAAAAZbk/-izm7aMM1ioyT29YNss-FclB8zmYOUCBgCLcBGAsYHQ/s1600/Capture1.PNG)

### Advance usage

Show report with a custom form.
```cs
// defines you report form
public class YourReportForm : Form, IReportWindow
{
    ....
    public void Initialize(Exception exception, string productName, string devMail)
    {
        textBox1.Text = exception.Message;
        textBox2.Text = productName;
        textBox3.Text = devMail;
    }
}

// Registers you report form
var appCrash = new AppCrash
{
    ProductName = "My product",
    DeveloperMail = "xuanson33bk@gmail.com"
};
appCrash.Register(typeof(YourReportForm));
```

Custom crash logs folder location
```cs
var appCrash = new AppCrash {CrashDir = @"C:\newCrashLogsDir"};
```

## IoC

Inversion of Control Container (bases on [https://github.com/grumpydev/TinyIoC](https://github.com/grumpydev/TinyIoC))

### Basic usage

```cs
// You can annotate your class by Component, Service, Repository or Controller, they are the same but for easier to understand their roles.
[Controller]
public class MyApp
{
    // Properties which are annotated with Autowired attribute will
    // be injected automatically
    [Autowired]
    public MyComponent MyComponent {get; set;}

    // myService will be injected automatically
    public MyApp(MyService myService) {...}
    ....
}
[Service]
public class MyService {....}
[Component]
public class MyComponent{....}

// Somewhere else...
// App32Context will scan the entry assembly to find classes which are annotated with Controller, Component, Repository or Service attributes.
using (var context = new App32Context())
{
    var myApp = context.Resolve<MyApp>();
    myApp.Start();
}
```

### Advance usage

Scans classes in specific assemblies
```cs
// If you don't pass any assembly, the entry assembly will be used
using (var context = new App32Context(assembly1, assemly2, assembly3))
{
    ....
}
```

Creates multiple instances of components
```cs
// By default, only one instance of a component will be created (singleton mode). If you want to create a new instance of a component each time it's injected to another component, set Singleton to false
[Service(Singleton = false)]
public class MyService {....}
```

IConfigLoader, IEventBus and AppCrash are registered automatically so you don't need to worry about these guys
```cs
using (var context = new App32Context())
{
    var config = context.Resolve<IConfigLoader>();
    ....
}
```

## DynamicInvoker

Manipulates on types which don't need to reference explicitly.

### Basic usage

Gets type
```cs
// Gets Control class from winform namespace, if current project doesn't reference to winform, return type will be null
var type = DynamicInvoker.GetType("System.Windows.Forms", "Control");
```

Registers event
```cs
// Registers ApplicationExit event on Application class
var type = DynamicInvoker.GetType("System.Windows.Forms", "Application");
if (type != null)
{
    DynamicInvoker.AddEventHandler<EventHandler>(type, "ApplicationExit", Application_ApplicationExit);
}
```

Invokes method
```cs
// Invokes CreateControl method on Control instance
var type = DynamicInvoker.GetType("System.Windows.Forms", "Control");
if (type != null)
{
    var control = Activator.CreateInstance(type);
    DynamicInvoker.InvokeMethod(control, "CreateControl");
}
```

Gets or sets property value
```cs
// Gets ProductName from Application class
var type1 = DynamicInvoker.GetType("System.Windows.Forms", "Application");
if (type1 != null)
{
    var productName = DynamicInvoker.GetPropertyValue(type1, "ProductName") as string;
}

// Sets "My text" to Text property
var type2 = DynamicInvoker.GetType("System.Windows.Forms", "Control");
if (type2 != null)
{
    var control = Activator.CreateInstance(type2);
    DynamicInvoker.SetPropertyValue(control, "Text", "My text");
}
```

## Intercomm

Inter process communication which allows processes to communicate each other and synchronize their actions

### Basic usage

1 client - 1 server communication

Sender process (client)
```cs
using (var sender = new Sender("my-intercomm"))
{
  var response = await sender.SendAsync<string>("Hi server, I'm client1");
  Console.WriteLine("Response from the other process is: " + response);
}
```

Receiver process (server)
```cs
using (var receiver = new SingleReceiver("my-intercomm"))
{
  var request = await receiver.ReceiveAsync<string>();
  await receiver.SendAsync("Hello " + request);
}
```

### Advantage usage

n client - 1 server communication

Setup your sender process likes the basic usage section.

Receiver process (server)
```cs
using (var receiver = new MultiReceiver("my-intercomm"))
{
    await receiver.WaitForSessionAsync(async session =>
    {
        var request = await session.ReceiveAsync<string>();
        await session.SendAsync("Hello " + request);
    });
}
```

Currently this library supports communicating through tcp and pipe:
- Blackcat.Intercomm.Pipe: for pipe supported
- Blackcat.Intercomm.Tcp: for tcp supported
