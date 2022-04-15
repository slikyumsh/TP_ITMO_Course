package com.example.services;

import com.example.repository.TaskRepository;
import com.example.entity.TaskEntity;
import com.example.entity.UserEntity;
import com.example.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class TaskService {
    @Autowired
    private TaskRepository taskRepo;
    @Autowired
    private UserRepository userRepo;

    public TaskEntity createTask(TaskEntity task, Long userId) {
        UserEntity user = userRepo.findById(userId).get();
        task.setUser(user);
        return task;
    }

    public TaskEntity complete(Long id) {
        TaskEntity task = taskRepo.findById(id).get();
        task.setCompleted(!task.getCompleted());
        return task;
    }
}