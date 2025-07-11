using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuickTimeEvent : MonoBehaviour
{
    public event Action OnSuccess; // Успех
    public event Action OnFail;    // Провал

    [Header("Settings")]
    [SerializeField] private GameObject qtePointPrefab; // Префаб точки
    [SerializeField] private Transform pointParent;     // Родительский объект для точек (например, Canvas)
    [SerializeField] private Vector2 screenPadding = new Vector2(100, 100); // Отступы от краёв экрана
    [SerializeField] private int numberOfPoints = 3; // Количество точек
    [SerializeField] private float qteDelay = 3f; // Таймер для каждой точки
    [SerializeField] private float touchRadius = 100f; // Радиус касания
    [SerializeField] private float minDistanceBetweenPoints = 150f; // Минимальное расстояние между точками
    [SerializeField] private int maxPlacementAttempts = 50; // Максимальное количество попыток размещения точки
    
    [SerializeField] private GameObject bgPanel;

    private InputManager inputManager;
    private GameObject currentPoint;
    private RectTransform currentPointRect;
    private Image currentPointImage;
    private float currentPointTimer;
    private int completedPoints = 0;
    private bool qteActive = false;
    private Canvas canvas;
    private Camera uiCamera;
    private List<Vector2> usedPositions = new List<Vector2>(); // Позиции уже использованных точек
    
    // Переменные для анимации уменьшения спрайта
    private Vector3 initialScale;
    private Vector3 targetScale;

    void Start()
    {
        inputManager = GameManager.Instance.InputManager;
        if (inputManager == null)
        {
            Debug.LogError("InputManager не найден!");
            return;
        }

        // Получаем Canvas и камеру
        canvas = pointParent.GetComponentInParent<Canvas>();
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            uiCamera = canvas.worldCamera;
        }
        bgPanel?.SetActive(false);
        //StartQTE();
    }

    void Update()
    {
        if (!qteActive) return;

        UpdateCurrentPoint();
        HandleInput();
    }

    public void StartQTE()
    {
        bgPanel?.SetActive(true);
        if (qteActive) return;
        Time.timeScale = 0.5f;
        qteActive = true;
        completedPoints = 0;
        usedPositions.Clear();
        CreateNextPoint();
    }

    private void CreateNextPoint()
    {
        if (completedPoints >= numberOfPoints)
        {
            // Все точки завершены
            bgPanel?.SetActive(false);
            Debug.Log("Все точки успешно нажаты!");
            OnSuccess?.Invoke();
            qteActive = false;
            Time.timeScale = 1f;
            return;
        }

        // Создаем новую точку
        currentPoint = Instantiate(qtePointPrefab, pointParent);
        currentPointRect = currentPoint.GetComponent<RectTransform>();
        currentPointImage = currentPoint.GetComponent<Image>();
        
        // Устанавливаем якорь в центр для правильного позиционирования
        currentPointRect.anchorMin = Vector2.zero;
        currentPointRect.anchorMax = Vector2.zero;

        // Находим подходящую позицию
        Vector2 newPosition = FindValidPosition();
        currentPointRect.anchoredPosition = newPosition;
        usedPositions.Add(newPosition);

        // Инициализируем таймер и масштаб
        currentPointTimer = qteDelay;
        
        // Сохраняем изначальный масштаб и устанавливаем целевой масштаб
        initialScale = currentPointRect.localScale;
        targetScale = Vector3.zero; // Уменьшаем до нуля
        
        // Сбрасываем fillAmount, если используется Image типа Filled
        if (currentPointImage != null)
        {
            currentPointImage.fillAmount = 1f;
        }

        Debug.Log($"Создана точка {completedPoints + 1}/{numberOfPoints} в позиции: {newPosition}");
    }

    private Vector2 FindValidPosition()
    {
        // Получаем размеры Canvas
        RectTransform canvasRect = pointParent as RectTransform;
        if (canvasRect == null && canvas != null)
            canvasRect = canvas.GetComponent<RectTransform>();

        float canvasWidth = canvasRect != null ? canvasRect.rect.width : Screen.width;
        float canvasHeight = canvasRect != null ? canvasRect.rect.height : Screen.height;

        // Получаем размер точки
        float pointSize = touchRadius; // Используем touchRadius как размер точки
        if (currentPointRect != null && currentPointRect.rect.width > 0)
        {
            pointSize = Mathf.Max(currentPointRect.rect.width, currentPointRect.rect.height);
        }

        // Границы с учетом размера точки
        float minX = screenPadding.x + pointSize / 2;
        float maxX = canvasWidth - screenPadding.x - pointSize / 2;
        float minY = screenPadding.y + pointSize / 2;
        float maxY = canvasHeight - screenPadding.y - pointSize / 2;

        Vector2 newPosition = Vector2.zero;
        bool validPositionFound = false;
        int attempts = 0;

        while (!validPositionFound && attempts < maxPlacementAttempts)
        {
            // Генерируем случайную позицию
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            newPosition = new Vector2(x, y);

            // Проверяем расстояние от уже использованных позиций
            validPositionFound = true;
            foreach (Vector2 usedPos in usedPositions)
            {
                float distance = Vector2.Distance(newPosition, usedPos);
                if (distance < minDistanceBetweenPoints)
                {
                    validPositionFound = false;
                    break;
                }
            }

            attempts++;
        }

        if (!validPositionFound)
        {
            Debug.LogWarning($"Не удалось найти оптимальную позицию после {maxPlacementAttempts} попыток. Используется позиция: {newPosition}");
        }

        return newPosition;
    }

    private void UpdateCurrentPoint()
    {
        if (currentPoint == null || currentPointRect == null) return;

        // Обновляем таймер
        currentPointTimer -= Time.deltaTime;

        // Вычисляем прогресс времени (от 1 до 0)
        float timeProgress = currentPointTimer / qteDelay;
        
        // Интерполируем масштаб от изначального к целевому
        Vector3 currentScale = Vector3.Lerp(targetScale, initialScale, timeProgress);
        currentPointRect.localScale = currentScale;

        // Проверяем, истекло ли время
        if (currentPointTimer <= 0)
        {
            Debug.LogWarning("Время истекло для точки!");
            OnFail?.Invoke();
            ResetQTE();
        }
    }

    private void HandleInput()
    {
        if (inputManager.TouchScreen())
        {
            Vector2 touchPos = inputManager.TouchPosition();
            CheckTouch(touchPos);
        }
    }

    private void CheckTouch(Vector2 touchPosition)
    {
        if (currentPoint == null || currentPointRect == null) return;

        // Конвертируем позицию точки из локальных координат в экранные
        Vector2 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(uiCamera, currentPointRect.position);
        
        // Сравниваем позиции в экранных координатах
        float distance = Vector2.Distance(touchPosition, targetScreenPosition);
        
        Debug.Log($"Touch: {touchPosition}, Target: {targetScreenPosition}, Distance: {distance}");

        if (distance <= touchRadius)
        {
            Debug.Log($"Попал по точке {completedPoints + 1}");
            
            // Удаляем текущую точку
            if (currentPoint != null)
            {
                Destroy(currentPoint);
                currentPoint = null;
                currentPointRect = null;
                currentPointImage = null;
            }
            
            completedPoints++;
            
            // Создаем следующую точку
            CreateNextPoint();
        }
        else
        {
            Debug.LogWarning($"Промах! Дистанция: {distance}");
            OnFail?.Invoke();
            ResetQTE();
        }
    }

    private void ResetQTE()
    {
        qteActive = false;
        bgPanel?.SetActive(false);
        
        // Удаляем текущую точку если есть
        if (currentPoint != null)
        {
            Destroy(currentPoint);
            currentPoint = null;
            currentPointRect = null;
            currentPointImage = null;
        }

        completedPoints = 0;
        usedPositions.Clear();
        
        Time.timeScale = 1f;
    }
}