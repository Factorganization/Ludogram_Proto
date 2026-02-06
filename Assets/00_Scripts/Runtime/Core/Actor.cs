using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Core/Actor")]
public class Actor : MonoBehaviour
{
    // cached shortcuts
    protected Transform ATransform;
    protected GameObject AGameObject;
    
    

    // unique id utile pour le debug / serialization / pooling
    private string _uniqueId;
    public string UniqueId => _uniqueId;
    public bool IsActive => gameObject.activeInHierarchy;

    // simple cache de composants par type
    private readonly Dictionary<Type, Component> _componentCache = new();

    // lifecycle flag pour BeginPlay
    private bool _hasBegunPlay;

    protected virtual void Awake()
    {
        ATransform = transform;
        AGameObject = gameObject;
        _uniqueId = Guid.NewGuid().ToString("N");
    }

    protected virtual void OnValidate()
    {
        // Garder les références valides dans l'éditeur
        ATransform = transform;
        AGameObject = gameObject;
    }

    protected virtual void Start()
    {
        if (!_hasBegunPlay)
        {
            _hasBegunPlay = true;
            BeginPlay();
        }
    }

    protected virtual void OnDestroy()
    {
        // appeler EndPlay pour nettoyage
        if (_hasBegunPlay)
        {
            EndPlay();
            _hasBegunPlay = false;
        }
        _componentCache.Clear();
    }

    // Héritables comme AActor::BeginPlay / EndPlay
    protected virtual void BeginPlay() { }
    protected virtual void EndPlay() { }

    // Utilitaire: get or add component
    public T GetOrAddComponent<T>() where T : Component
    {
        var t = GetComponent<T>();
        if (t == null) t = gameObject.AddComponent<T>();
        CacheComponent(t);
        return t;
    }

    // Récupération avec cache
    public T GetCachedComponent<T>() where T : Component
    {
        var type = typeof(T);
        if (_componentCache.TryGetValue(type, out var comp))
            return comp as T;

        var found = GetComponent<T>();
        if (found != null) _componentCache[type] = found;
        return found;
    }

    private void CacheComponent(Component c)
    {
        if (c == null) return;
        _componentCache[c.GetType()] = c;
    }

    // Wrapper d'activation pour contrôler BeginPlay/EndPlay si nécessaire
    public void SetActive(bool active)
    {
        if (gameObject.activeSelf == active) return;

        gameObject.SetActive(active);

        // si activation et Start n'a pas encore déclenché BeginPlay, le lancer
        if (active && !_hasBegunPlay && Application.isPlaying)
        {
            _hasBegunPlay = true;
            BeginPlay();
        }
        else if (!active && _hasBegunPlay && Application.isPlaying)
        {
            EndPlay();
            _hasBegunPlay = false;
        }
    }

    

    // Gizmos pour repérage dans l'éditeur
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(ATransform != null ? ATransform.position : transform.position, 0.25f);
    }
}
