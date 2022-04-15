package com.example.controllers;

import com.example.entity.UserEntity;
import com.example.services.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/user")
public class UserController {

    @Autowired
    private UserService userService;

    @PostMapping
    public UserEntity registration(@RequestBody UserEntity user) {
        try {
            return userService.createUser(user);
        } catch (RuntimeException e) {
            throw new RuntimeException(e);
        }
    }

    @GetMapping
    public UserEntity getOneUser(@RequestParam Long id) {
        try {
            return userService.getOne(id);
        } catch (RuntimeException e) {
            throw new RuntimeException(e);
        }
    }

    @GetMapping(value = "/all")
    public List<UserEntity> getAll() {
        try {
            return (List<UserEntity>) userService.getAll();
        } catch (RuntimeException e) {
            throw new RuntimeException(e);
        }
    }

    @DeleteMapping(value = "/{id}")
    public Long deleteUser(@PathVariable Long id) {
        try {
            return userService.delete(id);
        } catch (RuntimeException e) {
            throw new RuntimeException(e);
        }
    }
}
