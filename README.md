# PathFindingVisualisation
Визуализатор алгоритмов поиска пути на основе WPF с MVVM подходом и консольной версии

## Описание
Этот проект представляет собой интерактивную визуализацию различных алгоритмов поиска пути. Пользователи могут наблюдать за процессом работы алгоритма в реальном времени и экспериментировать с различными параметрами.
Проект направлен в первую очередь на демонстрацию работы разных алгоритмов поиска.
![image](https://github.com/user-attachments/assets/db662fdf-d34c-47ca-b740-9ac01bea114b)

## Особенности
- Визуализация алгоритмов A*, Dijkstra и BFS, жадный поиск
- Добавление и удаление препятствий
- Изменение начальной и конечной точки
- Настройка скорости анимации поиска

## Требования к окружению
- .NET 6 или выше
- Visual Studio 2022 или новее
- Windows 10 или новее

## Установка
```bash
git clone https://github.com/Mideks/PathFindingVisualisation.git
cd PathFindingVisualisation
```

## Сборка и запуск
1. Откройте решение в Visual Studio
2. Восстановите NuGet пакеты
3. Установите стартовый проект. PathFindingVisualisation - WPF версия, PathFindongConsole - консольная версия. 
4. Запустите программу (F5)

## Использование
- ПКМ / ЛКМ: Добавление или удаление препятствие
- Перетаскиванием можно менять положение стартовой(жёлтой) и конечной(красной) точки.
- Начать поиск: запуск визуализации 

## Пояснения для визуализации (WPF)
- Жёлтая клетка - начало
- Красная клетка - конец
- Чёрная клетка - препятствие
- Белая клетка - пустая клетка
- Зелёная клетка (после запуска) - найденный путь
- Синяя клетка (после запуска) - исследованная клетка
- Голубая клетка (после запуска) - клетки в очереди на исследование

## Вклад
Прожект открыт для участия. Создавайте pull requests для внесения изменений.

## Лицензия
MIT License
