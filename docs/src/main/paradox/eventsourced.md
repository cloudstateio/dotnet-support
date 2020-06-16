# Event sourcing

This page documents how to implement Cloudstate event sourced entities in .Net C#. For information on what Cloudstate event sourced entities are, please read the general @extref:[Event sourcing](cloudstate:user/features/eventsourced.html) documentation first.

An event sourced entity can be created by annotating it with the `[EventSourcedEntity]` attribute.

@@snip [EventSourcedEntity.cs]($base$/docs/src/test/eventsourced/EventSourcedEntity.cs) { #entity-class }

## Persistence types and serialization

Event sourced entities persist events and snapshots, and these need to be serialized when persisted. The most straight forward way to persist events and snapshots is to use protobufs. Cloudstate will automatically detect if an emitted event is a protobuf, and serialize it as such. For other serialization options, including JSON, see @ref:[Serialization](serialization.md).

While protobufs are the recommended format for persisting events, it is recommended that you do not persist your services protobuf messages, rather, you should create new messages, even if they are identical to the services. While this may introduce some overhead in needing to convert from one type to the other, the reason for doing this is that it will allow the services public interface to evolve independently from its data storage format, which should be private.

For our shopping cart example, we'll create a new file called `domain.proto`, the name domain is selected to indicate that these are my applications domain objects:

@@snip [domain.proto]($base$/docs/src/test/proto/domain.proto)

## State

Each entity should store its state locally in a mutable variable, either a mutable field or a multiple structure such as a collection. For our shopping cart, the state is a dictionary of product ids to products, so we'll create a dictionary to contain that:

@@snip [EventSourcedEntity.cs]($base$/docs/src/test/eventsourced/EventSourcedEntity.cs) { #entity-state }

## Constructing

While you don't necessarily need to define a constructor, you can define one and have that context and EntityId injected in. Use the `[EntityId]` attribute to indicate where the EntityId value will be set.

The constructor below shows having the entity id injected:

@@snip [EventSourcedEntity.cs]($base$/docs/src/test/eventsourced/EventSourcedEntity.cs) { #constructing }

## Handling commands

Command handlers can be declared by annotating a method with `[CommandHandler]` attribute. They take a context class of type `ICommandContext`.

By default, the name of the command that the method handles will be the name of the method with the first letter capitalized. So, a method called `GetCart` will handle gRPC service call command named `GetCart`.

The return type of the command handler must be the output type for the gRPC service call, this will be sent as the reply.

The following shows the implementation of the `GetCart` command handler. This command handler is a read-only command handler, it doesn't emit any events, it just returns some state:

@@snip [EventSourcedEntity.cs]($base$/docs/src/test/eventsourced/EventSourcedEntity.cs) { #get-cart }

### Emitting events

Commands that modify the state may do so by emitting events.

@@@ warning
The **only** way a command handler may modify its state is by emitting an event. Any modifications made directly to the state from the command handler will not be persisted, and when the entity is passivated and next reloaded, those modifications will not be present.
@@@

A command handler may emit an event by taking in a `ICommandContext` parameter, and invoking the `emit` method on it. Invoking `emit` will immediately invoke the associated event handler for that event - this both validates that the event can be applied to the current state, as well as updates the state so that subsequent processing in the command handler can use it.

Here's an example of a command handler that emits an event:

@@snip [EventSourcedEntity.cs]($base$/docs/src/test/eventsourced/EventSourcedEntity.cs) { #add-item }

This command handler also validates the command, ensuring the quantity items added is greater than zero. Invoking `ctx.fail` fails the command - this method throws so there's no need to explicitly throw an exception.

## Handling events

Event handlers are invoked at two points, when restoring entities from the journal, before any commands are handled, and each time a new event is emitted. An event handlers responsibility is to update the state of the entity according to the event. Event handlers are the only place where its safe to mutate the state of the entity at all.

Event handlers are declared by annotating a method with ` [EventHandler(typeof(Type))]` attribute. They take a context class of type `IEventBehaviorContext`.

Event handlers are differentiated by the type of event they handle in `EventHandler` attribute.

Here's an example event handler for the `ItemRemoved` event.

@@snip [EventSourcedEntity.cs]($base$/docs/src/test/eventsourced/EventSourcedEntity.cs) { #item-removed }

## Producing and handling snapshots

Snapshots are an important optimisation for event sourced entities that may contain many events, to ensure that they can be loaded quickly even when they have very long journals. To produce a snapshot, a method annotated with `[Snapshot]` attribute must be declared. It takes a context class of type `ISnapshotContext`, and must return a snapshot of the current state in serializable form. 

@@snip [EventSourcedEntity.cs]($base$/docs/src/test/eventsourced/EventSourcedEntity.cs) { #snapshot }

When the entity is loaded again, the snapshot will first be loaded before any other events are received, and passed to a snapshot handler. Snapshot handlers are declare by annotating a method with `[SnapshotHandler]` attribute, and it can take a context class of type `ISnapshotContext`.

Multiple snapshot handlers may be defined to handle multiple different types of snapshots, the type matching is done in the same way as for events.

@@snip [EventSourcedEntity.cs]($base$/docs/src/test/eventsourced/EventSourcedEntity.cs) { #handle-snapshot }

## Registering the entity

Once you've created your entity, you can register it with the `CloudState` server, by invoking the `RegisterEventSourcedEntity<EntityType>` method. In addition to passing the protobuf descriptors and any other additional descriptors you use.

@@snip [Program.cs]($base$/docs/src/test/eventsourced/Program.cs) { #register }

The complete code for our entity class would look like this:

@@snip [ShoppingCartEntity.cs]($base$/docs/src/test/eventsourced/ShoppingCartEntity.cs) { #entity-content }
