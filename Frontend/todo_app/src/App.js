import React, { Component } from 'react';
import toastr from 'toastr';
import 'toastr/build/toastr.min.css';
import { fromJS } from 'immutable';
import { DragDropContext, Droppable } from 'react-beautiful-dnd';
import './style.scss';
import Column from './components/Column/';
import AddNewModal from './components/AddNewModal/';
import Task from './components/Task/';
import '@fortawesome/fontawesome-free/css/all.min.css';

class App extends Component {
  state = {
    displayModal: false,
    editingColumnIndex: '',
    taskContent: '',
    editingTaskIndex: null,
    editedTaskId: null,
    columns: fromJS([
      { id: 'td', title: 'TO DO', tasks: [] },
      { id: 'ip', title: 'IN PROGRESS', tasks: [] },
      { id: 'de', title: 'DONE', tasks: [] }
    ])
  }

  componentDidMount() {
    fetch(`${process.env.REACT_APP_API_ENDPOINT}`)
      .then(response => response.json())
      .then(data => {
        const columns = this.state.columns.map(column => {
          const status = column.get('id') === 'td' ? 0 : column.get('id') === 'ip' ? 1 : 2;
          const tasks = data.data.filter(task => task.status === status).map(task => ({
            id: task.id,
            content: task.name,
            time: new Date(task.createAt).toLocaleString()
          }));
          return column.set('tasks', fromJS(tasks));
        });
        this.setState({ columns });
      })
      .catch(error => {
        console.error('Error fetching data:', error);
        toastr.error('Error fetching data from API', 'Error', { timeOut: 2000 });
      });

  }

  handleToggleModal = (choosenColumn = '') => () => {
    this.setState(prevState => ({
      displayModal: !prevState.displayModal,
      editingColumnIndex: choosenColumn
    }));
  }

  handleChangeTaskContent = (e) => this.setState({ taskContent: e.target.value });

  handleChangeeditingColumnIndex = (editingColumnIndex) => () => this.setState({ editingColumnIndex });

  handleAddNewTask = () => {
    const { taskContent, selectedColumn } = this.state;
    if (taskContent.trim() === '') {
      toastr.warning('Please enter your task', 'Notice', { timeOut: 2000 });
    } else {
      // Kiểm tra nếu cột hiện tại không phải là "TO DO" thì đặt trạng thái mặc định là "TO DO"
      const status = selectedColumn === 'td' ? 0 : selectedColumn === 'ip' ? 1 : selectedColumn === 'de' ? 2 : 0; // Đặt trạng thái mặc định là "TO DO" nếu không phải là "TO DO"
      const newTask = {
        name: taskContent,
        description: '',
        status: status,
        createAt: new Date().toISOString(),
        updateAt: new Date().toISOString()
      };
      fetch(`${process.env.REACT_APP_API_ENDPOINT}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newTask)
      })
        .then(response => {
          if (!response.ok) {
            throw new Error('Failed to add new task');
          }
          return response.json();
        })
        .then(() => {
          toastr.success('New task added successfully', 'Success', { timeOut: 2000 });
          this.fetchData();  // Load lại dữ liệu từ API
          this.handleToggleModal()(); // Ẩn modal sau khi thêm thành công
        })
        .catch(error => {
          console.error('Error adding new task:', error);
          toastr.error('Error adding new task', 'Error', { timeOut: 2000 });
        });
      window.location.reload();
    }
  }


  handleDeleteTask = (columnIndex, taskIndex) => () => {
    const result = window.confirm('Are your sure to delete this task?');
    if (result) {
      const { columns } = this.state;
      const task = columns.getIn([columnIndex, 'tasks', taskIndex]);
      fetch(`${process.env.REACT_APP_API_ENDPOINT}/${task.get('id')}`, {
        method: 'DELETE'
      })
        .then(() => {
          const updatedColumn = columns.updateIn([columnIndex, 'tasks'], tasks => tasks.remove(taskIndex));
          this.setState({ columns: fromJS(updatedColumn) }, () => {
            toastr.success('Delete task success', 'Notice', { timeOut: 2000 });
          });
        })
        .catch(error => {
          console.error('Error deleting task:', error);
          toastr.error('Error deleting task', 'Error', { timeOut: 2000 });
        });
    }
  }

  handleChooseEditTask = (columnIndex, taskIndex, taskId) => () => {
    this.setState({
      editingColumnIndex: columnIndex,
      editingTaskIndex: taskIndex,
      editedTaskId: taskId
    })
  }

  handleChangeSelectedColumn = (selectedColumn) => () => {
    this.setState({ selectedColumn });
  }

  handleEdit = () => {
    const { columns, editingColumnIndex, taskContent, editingTaskIndex } = this.state;
    const task = columns.getIn([editingColumnIndex, 'tasks', editingTaskIndex]);
    const updatedTask = {
      ...task.toJS(),
      name: taskContent,
      updateAt: new Date().toISOString()
    };

    fetch(`${process.env.REACT_APP_API_ENDPOINT}/${task.get('id')}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(updatedTask)
    })
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to update task');
        }
        return response.json();
      })
      .then(() => {
        toastr.success('Task updated successfully', 'Success', { timeOut: 2000 });
        const updatedColumns = columns.updateIn(
          [editingColumnIndex, 'tasks', editingTaskIndex],
          () => fromJS(updatedTask)
        );
        this.setState({ columns: updatedColumns });
      })
      .catch(error => {
        console.error('Error updating task:', error);
        toastr.error('Error updating task', 'Error', { timeOut: 2000 });
      });
      window.location.reload();
  }


  handleCancelEdit = () => {
    this.setState({
      editingColumnIndex: '',
      taskContent: '',
      editedTaskId: null,
      editingTaskIndex: null
    });
  }

  handleSaveDrag = (result) => {
    const { source, destination, reason } = result;
    if (reason === 'DROP' && destination) {
      const { columns } = this.state;
      const sourceColumnIndex = columns.findIndex(column => column.get('id') === source.droppableId);
      const destinationColumnIndex = columns.findIndex(column => column.get('id') === destination.droppableId);
      const task = columns.getIn([sourceColumnIndex, 'tasks', source.index]);

      const statusMap = { 'td': 0, 'ip': 1, 'de': 2 };
      const updatedStatus = statusMap[destination.droppableId];

      // Tạo một bản sao của nhiệm vụ và chỉ cập nhật các trường cần thiết
      const updatedTask = {
        name: task.get('content'),
        status: updatedStatus
      };

      fetch(`https://localhost:7260/api/todo/${task.get('id')}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(updatedTask)
      })
        .then(response => {
          if (!response.ok) {
            throw new Error('Failed to update task');
          }
          return response.json();
        })
        .then(() => {
          const updatedColumns = columns
            .updateIn([sourceColumnIndex, 'tasks'], tasks => tasks.splice(source.index, 1))
            .updateIn([destinationColumnIndex, 'tasks'], tasks => tasks.splice(destination.index, 0, task.set('status', updatedStatus)));

          this.setState({ columns: updatedColumns });
          toastr.success('Task status updated successfully', 'Success', { timeOut: 2000 });
        })
        .catch(error => {
          console.error('Error updating task status:', error);
          toastr.error('Error updating task status', 'Error', { timeOut: 2000 });
        });
      window.location.reload();
    }
  }
  render() {
    const { columns, displayModal, editingColumnIndex, taskContent, editedTaskId } = this.state;

    return (
      <div className="App">
        <h1 className="App__title">TO DO LIST</h1>
        <DragDropContext onDragEnd={this.handleSaveDrag}>
          <div className="App__content">
            {
              columns.map((column, columnIndex) => (
                <Column key={column.get('id')}
                  column={column}
                  handleAddNewTask={this.handleToggleModal}
                >
                  <Droppable droppableId={column.get('id')}>
                    {
                      provided => (
                        <div ref={provided.innerRef}
                          {...provided.droppableProps}
                          style={{ minHeight: '300px' }}
                        >
                          {
                            column.get('tasks').map((task, taskIndex) => (
                              <Task key={task.get('id')}
                                index={taskIndex}
                                isEditing={task.get('id') === editedTaskId}
                                handleChangeTaskContent={this.handleChangeTaskContent}
                                task={task}
                                handleEdit={this.handleEdit}
                                handleCancelEdit={this.handleCancelEdit}
                                handleChooseEditTask={this.handleChooseEditTask(columnIndex, taskIndex, task.get('id'))}
                                handleDeleteTask={this.handleDeleteTask(columnIndex, taskIndex)} />
                            ))
                          }
                          {provided.placeholder}
                        </div>
                      )
                    }
                  </Droppable>
                </Column>
              ))
            }
          </div>
        </DragDropContext>
        {
          displayModal &&
          <AddNewModal editingColumnIndex={editingColumnIndex}
            taskContent={taskContent}
            handleChangeTaskContent={this.handleChangeTaskContent}
            handleChangeeditingColumnIndex={this.handleChangeeditingColumnIndex}
            handleAddNewTask={this.handleAddNewTask}
            handleToggleModal={this.handleToggleModal()}
            selectedColumn={this.state.selectedColumn}
            handleChangeSelectedColumn={this.handleChangeSelectedColumn} />
        }
      </div>
    );
  }
}

export default App;
