using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Models;

namespace TestTodoApi
{
    public class TodoItemsControllerTests
    {

        private TodoContext GetInMemoryDBContext()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new TodoContext(options);

            context.TodoItems.Add(new TodoItem { Id = 1, Name = "item1", IsComplete = true });
            context.TodoItems.Add(new TodoItem { Id = 2, Name = "item2", IsComplete = true });
            context.SaveChanges();

            return context;
        }
         
        //método GET (obtener)
        [Fact]
        public async Task GetTodoItems_ReturnTodoItem_WhenItemExists()
        {
            // Arrange
            var context = GetInMemoryDBContext();
            var controller = new TodoItemsController(context);
            var itemId = 1;

            // Act
            var result = await controller.GetTodoItem(itemId);

            // Assert
            Assert.NotNull(result);
            var actionResult = Assert.IsType<ActionResult<TodoItem>>(result);
            Assert.Equal(itemId, actionResult.Value.Id);
            Assert.Equal("item1", actionResult.Value.Name);
        }

        //método POST (crear)
        [Fact]
        public async Task PostTodoItem_AddsNewItem_ReturnsCreatedAtAction()
        {
            // Arrange
            var context = GetInMemoryDBContext();
            var controller = new TodoItemsController(context);
            var newItem = new TodoItem { Id = 3, Name = "item3", IsComplete = false };

            // Act
            var result = await controller.PostTodoItem(newItem);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TodoItem>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<TodoItem>(createdAtActionResult.Value);
            Assert.Equal(newItem.Id, returnValue.Id);
            Assert.Equal("item3", returnValue.Name);
            Assert.False(returnValue.IsComplete);
        }

        //método PUT (actualizar)
        [Fact]
        public async Task PutTodoItem_UpdatesExistingItem_ReturnsNoContent()
        {
            // Arrange
            var context = GetInMemoryDBContext();
            var controller = new TodoItemsController(context);

            var updatedItem = new TodoItem { Id = 1L, Name = "updatedItem1", IsComplete = true };

            // Act
            var result = await controller.PutTodoItem(1L, updatedItem);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            var updatedTodoItem = await context.TodoItems.FindAsync(1L); // Id de tipo long
            Assert.Equal("updatedItem1", updatedTodoItem.Name);
            Assert.True(updatedTodoItem.IsComplete);
        }


        //método DELETE (eliminar)
        [Fact]
        public async Task DeleteTodoItem_DeletesItem_ReturnsNoContent()
        {
            // Arrange
            var context = GetInMemoryDBContext();
            var controller = new TodoItemsController(context);

            var itemId = 1L;

            // Act
            var result = await controller.DeleteTodoItem(itemId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var deletedItem = await context.TodoItems.FindAsync(itemId);
            Assert.Null(deletedItem);
        }

    }
}