### Интерфейс

rpd-component-examine-mode-details = Выбран режим: '{ $mode }'.
rpd-component-examine-build-details = Выбран режим строительства: { $name }.

# Смена мода
rpd-component-change-mode = РРТ переключён в режим '{ $mode }'.
rpd-component-change-build-mode = РРТ переключён в режим строительства. Строится { $name }.

# Кол-во материи
rpd-component-no-ammo-message = В инструменте закончились заряды!
rpd-component-insufficient-ammo-message = В инструменте не хватает зарядов!

# Разборка
rpd-component-deconstruct-target-not-on-whitelist-message = Вы не можете демонтировать это!
rpd-component-nothing-to-deconstruct-message = Здесь нечего демонтировать!

# Строительство
rpd-component-cannot-build-on-empty-tile-message = Это не может быть построено без фундамента.
rpd-component-must-build-on-subfloor-message = Это может быть построено только на покрытии!
rpd-component-cannot-build-on-occupied-tile-message = Здесь нельзя строить, место уже занято!

### Имя категорий

rpd-component-DisposalPipe = Утилизационные трубы
rpd-component-Gaspipes = Газовые трубы
rpd-component-Devices = Девайсы

### Дополнительная информация

rpd-component-deconstruct = Демонтаж

rpd-ammo-component-on-examine =
    Содержит:
    \n- { $rpdCharges } { $rpdCharges ->
        [one] заряд RPD
        [few] заряда RPD
       *[other] зарядов RPD
    }
    \n- { $rcdCharges } { $rcdCharges ->
        [one] заряд RCD
        [few] заряда RCD
       *[other] зарядов RCD
    }.

rpd-ammo-component-after-interact-full = Инструмент полон!
rpd-ammo-component-after-interact-refilled = Вы пополняете инструмент.