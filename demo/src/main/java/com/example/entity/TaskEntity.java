package com.example.entity;

import javax.persistence.*;

@Entity
public class TaskEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long taskId;
    private String taskName;
    private boolean completed;

    @ManyToOne
    private UserEntity user;

    public void setUser(UserEntity user) {
        this.user = user;
    }

    public boolean getCompleted() {
        return completed;
    }

    public void setCompleted(boolean completed) {
        this.completed = completed;
    }
}
