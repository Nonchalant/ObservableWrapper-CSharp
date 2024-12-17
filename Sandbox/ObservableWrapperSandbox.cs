using System;
using ObservableWrapperGenerator;
using R3;

namespace Sandbox
{
    public partial class SubjectClass
    {
        [ObservableWrapper] 
        private readonly Subject<int> _a = new Subject<int>();
        
        [ObservableWrapper] 
        private readonly Subject<string> _b = new Subject<string>();
        
        public void Test() {
            Console.WriteLine(A);
        }
    }
    
    public partial class BehaviorSubjectClass
    {
        [ObservableWrapper] 
        private readonly BehaviorSubject<int> _c = new BehaviorSubject<int>(0);
        
        public void Test() {
            Console.WriteLine(C);
        }
    }
}