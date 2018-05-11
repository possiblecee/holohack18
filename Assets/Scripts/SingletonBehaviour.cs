using UnityEngine;

/// <inheritdoc />
/// <summary>
/// Base class for persistent singleton types.
/// A singleton instance lives throughout the entire lifetime of the application.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Stored singleton instance.
    /// </summary>
    private static T _instance;

    /// <summary>
    /// State variable for indicating that the application is quitting.
    /// </summary>
    private static bool _isShuttingDown;

    /// <summary>
    /// Provides the single stored instance. If it doesn't exist, creates it.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_isShuttingDown)
            {
                Debug.LogWarning("[SingletonBehaviour] Accessing instance is prohibited at this point. Application is shutting down.");
                return null;
            }

            if (_instance == null)
            {
                _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    /// <summary>
    /// Returns true if the current instance is a duplicate about to be destroyed.
    /// </summary>
    protected bool IsDuplicate
    {
        get { return _instance != this; }
    }

    #region Unity Engine Callbacks

    /// <summary>
    /// Base needs to be called first if you override this.
    /// </summary>
    protected virtual void Awake()
    {
        // Single instance guarding
        if (_instance != null)
        {
            Debug.LogError("[SingletonBehaviour] " + gameObject.name +
                           " is a duplicate singleton instance that has been destroyed.");
            DestroyImmediate(gameObject);
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void OnDestroy()
    {
        _isShuttingDown = _instance == this;
    }

    #endregion
}