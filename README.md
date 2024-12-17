```CSharp
using ObservableWrapperGenerator;
using R3;

public partial class BeforeCode
{
    private readonly Subject<int> _number = new();
    public Observable<int> Number => _number.AsObservable();
    
    private readonly BehaviorSubject<string> _text = new("AAA");
    public Observable<int> Text => _text.AsObservable();
}

public partial class AfterCode
{
    [ObservableWrapper]
    private readonly Subject<int> _number = new();
    
    [ObservableWrapper] 
    private readonly BehaviorSubject<string> _text = new("AAA");
}
```

### Reference

- https://qiita.com/amenone_games/items/762cbea245f95b212cfa
- https://learning.unity3d.jp/10120/
