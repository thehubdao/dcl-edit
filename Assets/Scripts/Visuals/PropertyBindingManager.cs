using Assets.Scripts.System;
using Zenject;

public class PropertyBindingManager
{
    // Dependencies
    private SceneManagerSystem sceneManagerSystem;
    private CommandSystem commandSystem;

    [Inject]
    private void Construct(SceneManagerSystem sceneManagerSystem, CommandSystem commandSystem)
    {
        this.sceneManagerSystem = sceneManagerSystem;
        this.commandSystem = commandSystem;
    }

    public ValueBindStrategy<T> GetPropertyBinding<T>(DclPropertyIdentifier propertyIdentifier)
    {
        var concreteProperty = sceneManagerSystem.GetCurrentScene().GetPropertyFromIdentifier(propertyIdentifier).GetConcrete<T>();
        return new ValueBindStrategy<T>(
            value: () => concreteProperty.GetConcrete<T>().Value,
            onValueSubmitted: value =>
            {
                concreteProperty.ResetFloating();
                var oldValue = concreteProperty.Value;
                var command = commandSystem.CommandFactory.CreateChangePropertyCommand(propertyIdentifier, oldValue, value);

                commandSystem.ExecuteCommand(command);
            },
            onErrorSubmitted: _ => concreteProperty.ResetFloating(),
            onValueChanged: value => concreteProperty.SetFloatingValue(value),
            onErrorChanged: _ => concreteProperty.ResetFloating());
    }
}
