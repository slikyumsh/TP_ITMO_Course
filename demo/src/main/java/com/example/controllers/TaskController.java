package com.example.controllers;

import com.example.entity.TaskEntity;
import com.example.services.TaskService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/task")
public class TaskController {

    @Autowired
    private TaskService taskService;

    @PostMapping
    public TaskEntity createTodo(@RequestBody TaskEntity todo, @RequestParam Long userId) {
        try {
            return taskService.createTask(todo, userId);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @PutMapping
    public TaskEntity completeTodo(@RequestParam Long id) {
        try {
            return taskService.complete(id);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
}