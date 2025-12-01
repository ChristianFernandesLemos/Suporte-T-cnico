import React from 'react';
import { View, Text, TouchableOpacity, ScrollView } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import Sidebarstyles from '../styles/SidebarStyle';

// Recebe userId como prop vinda da WelcomeScreen
const Sidebar = ({ isOpen, onClose, navigation, userId }) => {
  const menuItems = [
    { icon: 'add-circle-outline', label: 'Registrar Chamado', screen: 'NewTicket' },
    { icon: 'eye-outline', label: 'Visualizar Chamados', screen: 'CallListScreen' },
  ];

  if (!isOpen) return null;

  return (
    <View style={Sidebarstyles.overlay}>
      <TouchableOpacity style={Sidebarstyles.backdrop} onPress={onClose} />
      <View style={Sidebarstyles.sidebar}>
        <View style={Sidebarstyles.header}>
          <Text style={Sidebarstyles.logo}>
            Inter<Text style={Sidebarstyles.logoHighlight}>Fix</Text>
          </Text>
          <TouchableOpacity onPress={onClose}>
            <Ionicons name="close" size={28} color="#000" />
          </TouchableOpacity>
        </View>

        <ScrollView style={Sidebarstyles.menuContainer}>
          {menuItems.map((item, index) => (
            <TouchableOpacity
              key={index}
              style={Sidebarstyles.menuItem}
              onPress={() => {
                // --- CORREÇÃO: Envia o objeto { idusuario: userId } ---
                navigation.navigate(item.screen, { idusuario: userId });
                onClose();
              }}
            >
              <Ionicons name={item.icon} size={24} color="#000" />
              <Text style={Sidebarstyles.menuText}>{item.label}</Text>
            </TouchableOpacity>
          ))}
          <TouchableOpacity 
            style={Sidebarstyles.logoutButton}
            onPress={() => {
                onClose();
                // Reseta a navegação para login
                navigation.reset({ index: 0, routes: [{ name: 'Login' }] });
            }}
        >
          <Ionicons name="log-out-outline" size={24} color="#d32f2f" />
          <Text style={Sidebarstyles.logoutText}>Sair</Text>
        </TouchableOpacity>
        </ScrollView>
      </View>
    </View>
  );
};

export default Sidebar;