using UnityEngine;
using UnityEngine.Events;
using UniRx;


public class EventWait : MonoBehaviour
{
    [SerializeField] private int frame = -1;
    [SerializeField] private UnityEvent unityEvent = null;

    public void Start() { if (1 <= frame) Invoke(frame); else if (frame == 0) Invoke(); }
    public void Invoke() => unityEvent.Invoke();
    public void Invoke(int frame) => Observable.TimerFrame(frame).Subscribe(_ => Invoke());
}
