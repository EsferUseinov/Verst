# Verst – VR/AR лабораторні роботи

Навчальний Unity-проєкт для курсу "Розроблення VR/AR застосунків".

## Структура гілок

- `main` – стабільна гілка, містить змердженні готові лабораторні роботи.
- `lab-1`, `lab-2`, ... – робочі гілки для кожної окремої лабораторної роботи.
  Після завершення роботи гілка мерджиться в `main`.

## Лабораторні роботи

1. **Лаб. 1** – Знайомство з Unity: 3D-сцена, префаби, скрипти та фізика колізій.
2. **Лаб. 2** – VR-сцена: пересування в просторі та маніпуляція об'єктами.
3. **Лаб. 3** – Просторові інтерфейси (Spatial UI) та покрокові форми.
4. **Лаб. 4** – Природні взаємодії руками (Hand Tracking).
5. **Лаб. 5** – Взаємодія з серверами: REST API та телеметрія.
6. **Лаб. 6** – Багатокористувацький режим (Photon) або Змішана реальність (Mixed Reality).

## Середовище

- Unity 6 LTS, Universal Render Pipeline (URP)
- Android Build Support (для збірок під Meta Quest)
- Meta XR All-in-One SDK / Unity XRI (з Лаб. 2)

## Сторонні ресурси

Текстури – з [ambientCG](https://ambientcg.com), ліцензія [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/)
(вільне використання без обов'язкового зазначення авторства). Використано версії 1K-JPG, карти Color і NormalGL.

| Ресурс | Де використано | Файли в проєкті |
|---|---|---|
| [Concrete 031](https://ambientcg.com/view?id=Concrete031) | підлога цеху | `Assets/_Project/Art/Textures/Floor/` |
| [Concrete 034](https://ambientcg.com/view?id=Concrete034) | стіни, стеля | `Assets/_Project/Art/Textures/Wall/` |
| [Wood 092](https://ambientcg.com/view?id=Wood092) | столи, стелаж, палета, ящик | `Assets/_Project/Art/Textures/Wood/` |
| [Metal Plates 004](https://ambientcg.com/view?id=MetalPlates004) | верстати (заглушки) | `Assets/_Project/Art/Textures/PaintedMetal/` |
