package com.example.services;

import com.example.entity.UserEntity;
import com.example.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class UserService {

    @Autowired
    private UserRepository userRepo;

    public UserEntity createUser(UserEntity user){
        return userRepo.save(user);
    }

    public UserEntity getOne(Long id) {
        return userRepo.findById(id).get();
    }

    public Iterable<UserEntity> getAll(){
        return userRepo.findAll();
    }

    public Long delete(Long id) {
        userRepo.deleteById(id);
        return id;
    }
}
