const baseAddress = "";

let app = new Vue({
  el: "#app",
  data: {
    appName: "Azure Functions ToDo List",
    todos: [],
    newTask: "",
    error: undefined,
  },
  created: function () {
    fetch(`${baseAddress}/api/todo`)
      .then((response) => response.json())
      .then((json) => {
        this.todos = json;
      })
      .catch((reason) => (this.error = `Failed to create todo: ${reason}`));
  },
  methods: {
    createTodo: function () {
      fetch(`${baseAddress}/api/todo`, {
        method: "POST",
        body: JSON.stringify({ description: this.newTask }),
      })
        .then((response) => response.json())
        .then((json) => {
          this.todos.push(json);
          this.newTask = "";
        })
        .catch((reason) => (this.error = `Failed to create todo: ${reason}`));
    },
    deleteTodo: function (todo) {
      fetch(`${baseAddress}/api/todo/${todo.id}`, {
        method: "DELETE",
      })
        .then((response) => response.json())
        .then((json) => {
          let index = this.todo.findIndex((element) => element.id == todo.id);
          this.todos.splice(index, 1);
          this.newTask = "";
        })
        .catch((reason) => (this.error = `Failed to create todo: ${reason}`));
    },
    completeTodo: function (todo) {
      fetch(`${baseAddress}/api/todo/${todo.id}`, {
        method: "PUT",
        body: JSON.stringify({ completed: !todo.completed }),
      })
        .then((response) => response.json())
        .then((json) => {})
        .catch((reason) => (this.error = `Failed to create todo: ${reason}`));
    },
  },
});
