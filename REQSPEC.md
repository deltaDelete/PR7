# Спецификация требований

## Содержание

- [1. Функциональные требования](#1функциональные-требования)
- [2. Нефункциональные требования](#2нефункциональные-требования)
- [3. Ограничения и требования к интерфейсу](#3ограничения-и-требования-к-интерфейсу)

## 1. Функциональные требования:

### 1. Возможность добавления записей пациентов в базу данных с указанием следующих параметров:

- Номер записи;
- Пациент;
- Лечащий врач;
- Тип записи;
- Выписанные препараты.

### 2. Записи имеют уникальный идентификатор.

### 3. Возможность редактирования записей:

- Изменение даты записи;
- Изменение типа записи;
- Изменение лечащего врача.

### 4. Возможность отслеживания статуса заявки:

- Отображение списка заявок;
- Получение уведомлений о смене статуса заявки;
- Поиск заявки по номеру или по параметрам.

### 5. Возможность возможность выписки пациенту лекарственных препаратов:

- Прикрепление лекарственного препарата к пациенту;
- Указание дозировки препарата;
- Указание частоты приема препарата.

### 6. Возможность выписки пациенту диагнозов:

- Диагноз выписывает врач;
- Врач вписывает дату диагноза;
- Врач вписывает тип диагноза;
- Врач вписывает заметку к диагнозу.

### 7. Расчет статистики:

- Количество приемов в день, неделю, месяц ;
- Расределение приемов в зависимости от дня недели;
- Статистика по назначенным препаратам (общее количество, средняя дозировка).

## 2. Нефункциональные требования:

### 1. Кросплатформенность:

- Поддержка работы на ОС семейства Windows;
- Поддержка работы на ОС семейства Linux (минимальная версия ядра 5.15).

### 2. Безопасность:

- Логин и пароль для доступа к приложению;
- Доступ к данным должен быть ограничен в зависимости от роли пользователя.

### 3. Удобство использования:

- Простой и интуитивный интерфейс;
- Информативные уведомления и подсказки.

### 4. Производительность:

- Приложение должно иметь быстрый доступ к данным;
- Минимальное время отклика на запросы пользователя.

## 3. Ограничения и требования к интерфейсу:

### 1. Требования к реализации:

- Язык программирование: C#;
- Объектно-реляционное отображение: Entity Framework
- СУБД: MySQL.

# ?Тестирование требований?

## **Системные характеристики**

- СХ-1: Приложение является графическим

- СХ-2: Для работы приложения используется среда выполнения .NET
    - Требуемая версия 7.x.x

- СХ-3: Приложение является кросспратформенным
    - Поддерживаемые ОС: Linux (X11, Wayland), Windows 10 и новее

## **Пользовательские требования**

- ПТ-1: Запуск и остановка приложения
    - Запуск приложения производиться с помощью исполняемого файла, запускаемого из графической среды
    - Остановка приложения производиться закрытием окна приложения

- ПТ-2: Конфигурирование приложение
    - Приложение не предоставляется возможностей конфигурации

- ПТ-3: Просмотр журнала работы приложения
    - Для просмотра журнала работы приложения, оно должно быть запущено с использованием командной строки

## **Бизнес-правила**

- БП-1: Источник данных:
    - Источником данных является база данных SQL

## **Атрибуты качества**

- АК-1: Производительность:
    - АК-1.1: Приложение должно обеспечивать скорость обработки данных, соответствующую скорости обработки данных
      используемой систему управления базой данных

- АК-2: Устойчивость к входным данным
    - АК-2.1: Вводимые пользователем данных должны соответствовать формату хранимому в базе данных
    - АК-2.2: Вводимые пользователей данные должны проверятся на соответствие форматам
    - АК-2.3: При несоответствии входимых пользователем данных требуемому формату, приложение должно предупредить
      пользователя о неверности этих данных и запретить их использование до исправления

## **Ограничения**

- О-1: Приложение разрабатывается на языке программирования C#
- О-2: Ограничения относительно версии и настроек среды выполнения .NET отражены в пункте ДС-1
  раздела [Детальные спецификации](#детальные-спецификации)

## **Детальные спецификации**

- ДС-1: Среда выполнения .NET
  - ДС-1.1: Минимальная версия -- 7.0
