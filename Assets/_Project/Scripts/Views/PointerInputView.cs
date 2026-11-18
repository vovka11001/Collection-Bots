using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerInputView : MonoBehaviour
{
    private const float MaxRayDistance = 1000f;

    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _clickMask;
    
    private readonly Subject<BaseView> _baseClicked = new Subject<BaseView>();
    private readonly Subject<Vector3> _groundClicked = new Subject<Vector3>();
    
    public Observable<BaseView> BaseClicked => _baseClicked;
    public Observable<Vector3> GroundClicked => _groundClicked;

    private void Awake()
    {
        _baseClicked.AddTo(this);
        _groundClicked.AddTo(this);
        
        Observable.EveryUpdate().Where(_ => IsLeftButtonPressedThisFrame()).Subscribe(_ => HandleClick())
            .AddTo(this);
    }

    private bool IsLeftButtonPressedThisFrame()
    {
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    }

    private void HandleClick()
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, MaxRayDistance, _clickMask) == false)
            return;
        
        BaseView baseview = hit.collider.GetComponentInParent<BaseView>();

        if (baseview != null)
        {
            _baseClicked.OnNext(baseview);
            return;
        }
        
        _groundClicked.OnNext(hit.point);
    }
}