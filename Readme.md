# AI Scriber

Интеллектуальная система генерации сценариев, фонов, персонажей и озвучки на основе заданной темы. Используется для автоматического создания мультимедийных историй с помощью ИИ.

---

## 🧩 Архитектура и сервисы

Система состоит из нескольких микросервисов, обменивающихся данными через **RabbitMQ**.

### 📥 CommandsReceiver
- Получает тему сценария от пользователя
- Отправляет задачу в очередь на генерацию

### ✍️ Scribe
- Генерация сценария, персонажей, фонов и озвучки
- Использует:
  - **ChatGPT** и **Mistral** для создания текста сценария и диалогов
  - **Stable Diffusion** и **Kadinsky** для генерации визуального контента (фоны и персонажи)
  - Инструменты синтеза речи для озвучивания

### 🎭 Streamer
- Запускает и проигрывает сгенерированный сценарий
- Объединяет визуальные и аудио-компоненты в готовый результат

---

## 🛠️ Технологии

- 🐇 **RabbitMQ** — для асинхронного взаимодействия между сервисами
- 🧠 **ChatGPT / Mistral** — генерация текстового контента
- 🎨 **Stable Diffusion / Kadinsky** — визуальное оформление
- 🔊 **TTS** — генерация озвучки

---

## 🚀 Быстрый старт

> В разработке. Инструкции по запуску будут добавлены позже.

---
