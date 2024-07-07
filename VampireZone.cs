using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class VampireZone : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private int _stealingHealth;
    [SerializeField] private float _radius;
    [SerializeField] private Color _changingColor;
    [SerializeField] private float _activatedTime;
    
    private Color _defaultColor;
    private Coroutine _coroutine;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
    }

    private void Update()
    {
        KeyCode key = KeyCode.RightControl;

        if (Input.GetKeyDown(key))
        {
            if (_coroutine == null)
            {
                _image.color = _changingColor;
                _coroutine = StartCoroutine(StealHealth());
            }
        }
    }

    private IEnumerator StealHealth()
    {
        WaitForSeconds waitTime = new WaitForSeconds(_activatedTime / _stealingHealth);
        
        Enemy lastEnemy;
        Enemy enemy;
        
        GetEnemy(out lastEnemy);

        for (int i = 0; i < _stealingHealth; i++)
        {
            yield return waitTime;

            GetEnemy(out enemy);

            if (enemy == null)
            {
                lastEnemy.TakeDamage(1);
            }
            else
            {
                lastEnemy = enemy;
                enemy.TakeDamage(1);
            }

            _player.Heal(1);
        }

        _image.color = _defaultColor;
        _coroutine = null;
    }

    private void GetEnemy(out Enemy enemy)
    {
        List<Collider2D> colliders = Physics2D.OverlapCircleAll(transform.position, _radius).ToList();
        List<Enemy> enemies = new List<Enemy>();

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.TryGetComponent(out Enemy enemy1))
            {
                enemies.Add(enemy1);
            }
        }

        var sortedEnemies = enemies.OrderBy(enemy => Vector2.Distance(enemy.transform.position, gameObject.transform.position));

        if (sortedEnemies.ToList().Count != 0)
        {
            enemy = sortedEnemies.ToList()[0];
        }
        else
        {
            enemy = null;
        }
    }
}