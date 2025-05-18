
# Создание 2D Roguelike-игры на Unity с механикой волн и улучшений

## Цель

Изучить процесс разработки 2D-игры в жанре roguelike с нуля, применяя C# и Unity, и воспроизвести практическую реализацию на одном уровне с прогрессирующими волнами противников и системой апгрейдов.


## 1. Исследование предметной области

### Что такое roguelike?

Roguelike — жанр, основанный на элементах процедурной генерации, перманентной смерти персонажа, волн врагов и системы улучшений.

### Ключевые особенности:

* Простая 2D-графика (сверху или сбоку)
* Постепенно усложняющиеся волны противников
* Апгрейды игрока за внутриигровую валюту
* Разнообразие врагов и механик

## 2. Технологии и инструменты

| Инструмент    | Назначение                                  |
| ------------- | ------------------------------------------- |
| Unity 2022+   | Игровой движок                              |
| C#            | Язык программирования логики                |
| Visual Studio | IDE для написания кода                      |

## 3. Пошаговое руководство

### Шаг 1: Инициализация проекта

* Открыть Unity Hub → Создать 2D-проект
* Настроить основную сцену: `Game`, `Canvas`, `EventSystem`
* Добавить слои: `Player`, `Enemy`, `UI`, `Projectiles`

### Шаг 2: Создание игрока

**Код движения:**

```csharp
public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    Vector2 movement;

    void Update() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate() {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
```

**Иллюстрация 1:** Спрайт игрока на сером фоне арены

![image](https://github.com/user-attachments/assets/c1d0f6ab-c4f4-4bdd-bed8-e4c33480850f)

### Шаг 3: Система стрельбы

```csharp
public GameObject bulletPrefab;
public Transform firePoint;

void Update() {
    if (Input.GetButtonDown("Fire1")) {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
```

**Иллюстрация 2:** Анимация выстрела в сторону курсора

![image](https://github.com/user-attachments/assets/caea44be-0cdc-4d25-bb18-30c504ed6b0f)

### Шаг 4: Враги и волны

**Spawner.cs**

```csharp
public GameObject[] enemies;
public float timeBetweenWaves = 15f;
private int wave = 0;

void Start() {
    InvokeRepeating(nameof(SpawnWave), 5f, timeBetweenWaves);
}

void SpawnWave() {
    wave++;
    for (int i = 0; i < wave * 3; i++) {
        Instantiate(enemies[Random.Range(0, enemies.Length)], GetRandomSpawnPoint(), Quaternion.identity);
    }
}
```

**Иллюстрация 3:** Первая волна из 3 противников

![image](https://github.com/user-attachments/assets/36b7221f-2c84-4af1-bb1c-3cbd373fbdd8)

### Шаг 5: Противники с разным поведением

**BasicEnemy.cs**

```csharp
public class BasicEnemy : MonoBehaviour {
    public float speed = 2f;
    private Transform player;

    void Start() {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update() {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }
}
```

**Иллюстрация 4:** Новый тип врага с другой системой атаки

![image](https://github.com/user-attachments/assets/0696a8f3-bc0c-4730-babe-57bf4ee64d60)

### Шаг 6: Сбор монет и покупки

**Coin.cs**

```csharp
public class Coin : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PlayerStats.money += 1;
            Destroy(gameObject);
        }
    }
}
```

**Shop UI (через Canvas):**

* Кнопка: «+ Урон»
* Кнопка: «Супер удар»
* Кнопка: «- КД»
* Кнопка: «Вампиризм»

**Иллюстрация 5:** Меню магазина с четырьмя апгрейдами

![image](https://github.com/user-attachments/assets/ac79bcff-f45c-4736-a836-33d5a4283ae4)

### Шаг 7: Реализация апгрейдов

**Пример: Урон**

```csharp
public class Weapon : MonoBehaviour {
    public int damage = 5;
    
    public void UpgradeDamage() {
        damage += 2;
    }
}
```

**Вампиризм:**

```csharp
public class Vampirism : MonoBehaviour {
    public int healPerKill = 1;

    public void OnEnemyKilled() {
        PlayerStats.hp += healPerKill;
    }
}
```

**Иллюстрация 6:** Игрок получает здоровье за убийство врага:

До:  
![image](https://github.com/user-attachments/assets/5ad92a1e-fe99-4a98-8612-923f9274ef16)

После:  
![image](https://github.com/user-attachments/assets/5e206d64-8108-4edf-b2e4-202f6d5e5e1e)

### Шаг 8: Система прогрессии

* С каждым раундом: больше врагов
* С интервалами: новые типы врагов
* Сложность растёт линейно или экспоненциально

**Иллюстрация 7:** Таблица: волна → количество врагов → типы врагов

| Волна | Количество врагов | Тип врага            |
| ------| ------------------|----------------------|
| 1-2   | 1-4               | Melee                |
| 3-4   | 5-8               | Melee + Ranged       |
| 5+    | 9-15              | Melee + Ranged + Boom|

### Шаг 9: UI и обратная связь

* Здоровье игрока (ProgressBar)
* Количество монет
* Анимации получения урона, выстрелов, убийства

**Иллюстрация 8:** Игровой интерфейс с жизнями и монетами

![image](https://github.com/user-attachments/assets/92b99abc-a1a2-4b97-8d99-e4be3ba26aef)

## 4. Дополнительные фичи (по желанию)

* Мини-карта
* Эффекты частиц при убийстве
* Система очков и Highscore
* Звук: выстрелы, попадания, смерть, покупки

**Иллюстрация 9:** Игрок убивает врага, появляются частицы и звук

![image](https://github.com/user-attachments/assets/b0a22034-c528-4670-bff0-07428ab392e2)

## Заключение

Это руководство охватывает процесс создания базового уровня 2D roguelike-игры в Unity. Использование системы волн, прогрессии врагов и апгрейдов — фундаментальные принципы жанра. Игра легко масштабируется за счёт добавления новых врагов, оружия, карт и уровней сложности.

