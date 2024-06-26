# LeoECS Lite Дополнительные системы.
Дополнительные системы, расширяющие функционал LeoECS Lite.

> **ВАЖНО!** АКТИВНАЯ РАЗРАБОТКА ПРЕКРАЩЕНА, ВОЗМОЖНО ТОЛЬКО ИСПРАВЛЕНИЕ ОБНАРУЖЕННЫХ ОШИБОК. СОСТОЯНИЕ СТАБИЛЬНОЕ, ИЗВЕСТНЫХ ОШИБОК НЕ ОБНАРУЖЕНО. ЗА НОВЫМ ПОКОЛЕНИЕМ ФРЕЙМВОРКА СТОИТ СЛЕДИТЬ В БЛОГЕ https://leopotam.com/

> Проверено на Unity 2020.3 (не зависит от Unity) и содержит asmdef-описания для компиляции в виде отдельных сборок и уменьшения времени рекомпиляции основного проекта.

# Содержание
* [Социальные ресурсы](#Социальные-ресурсы)
* [Установка](#Установка)
    * [В виде unity модуля](#В-виде-unity-модуля)
    * [В виде исходников](#В-виде-исходников)
* [Классы](#Классы)
    * [EcsGroupSystem](#EcsGroupSystem)
    * [DelHere](#DelHere)
* [Лицензия](#Лицензия)
* [ЧаВо](#ЧаВо)

# Социальные ресурсы
[Блог разработчика](https://leopotam.com/)

# Установка

## В виде unity модуля
Поддерживается установка в виде unity-модуля через git-ссылку в PackageManager или прямое редактирование `Packages/manifest.json`:
```
"com.leopotam.ecslite.extendedsystems": "https://github.com/Leopotam/ecslite-extendedsystems.git",
```
По умолчанию используется последняя релизная версия. Если требуется версия "в разработке" с актуальными изменениями - следует переключиться на ветку `develop`:
```
"com.leopotam.ecslite.extendedsystems": "https://github.com/Leopotam/ecslite-extendedsystems.git#develop",
```

## В виде исходников
Код так же может быть склонирован или получен в виде архива со страницы релизов.

# Классы

## EcsGroupSystem
`EcsGroupSystem` позволяет группировать ECS-системы в группы с возможностью включения / отключения в процессе работы: 
```c#
// Эти системы будут вложены в группу с именем "Melee".
class MeleeSystem1 : IEcsRunSystem {
    public void Run (IEcsSystems systems) { }
}
class MeleeSystem2 : IEcsRunSystem {
    public void Run (IEcsSystems systems) { }
}

class MeleeGroupEnableSystem : IEcsRunSystem {
    public void Run (IEcsSystems systems) {
        // Мы можем включать и выключать группу "Melee" с помощью
        // отправки специального события "EcsGroupSystemState".
        var world = systems.GetWorld ();
        var entity = world.NewEntity ();
        ref var evt = ref world.GetPool<EcsGroupSystemState> ().Add (entity);
        evt.Name = "Melee";
        evt.State = true;
    }
}
...
// Стартовая инициализация.
var systems = new EcsSystems (new EcsWorld ());
systems
    // Добавляем выключенную на старте группу "Melee" с 2 вложенными
    // системами, события изменения состояния группы будут
    // храниться в мире по умолчанию.
    .AddGroup ("Melee", false, null,
        new MeleeSystem1 (),
        new MeleeSystem2 ())
    // Остальные системы.
    .Add (new MeleeGroupEnableSystem ())
    .Init ();
```

## DelHere
`DelHere` - это вспомогательное поведение для автоматической очистки компонентов-событий в указанном месте:
```c#
var systems = new EcsSystems (new EcsWorld ());
systems
    .Add (new System1 ())
    .Add (new System2 ())
    // Все компоненты "C1" будут удалены здесь.
    .DelHere<C1> ()
    .Add (new System3 ())
    // Все компоненты "C2" будут удалены здесь.
    .DelHere<C2> ()
    .Add (new System4 ())
    .Init ();
```
> **ВАЖНО!** Если `DelHere` выполняет удаление компонентов в явно указанном мире - этот мир должен быть зарегистрирован через вызов `AddWorld()` до вызова `DelHere()`.

# Лицензия
Пакет выпускается под [MIT-Red лицензией](./LICENSE.md).

В случаях лицензирования по условиям MIT-Red не стоит расчитывать на
персональные консультации или какие-либо гарантии.

# ЧаВо

### Я хочу воспользоваться функционалом инъекции зависимостей из `ecslite-di`, но поля систем, вложенных в именованные группы, не инициализируются должным образом. Как это исправить?

Для этого достаточно указать директиву компилятора `LEOECSLITE_DI`.