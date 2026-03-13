import React, { useState } from 'react';
import { View, Text, SafeAreaView, TouchableOpacity } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
// Ajuste os caminhos conforme sua pasta components
import Header from './Header'; 
import Sidebar from './Sidebar'; 
import WelcomeStyle from '../styles/WelcomeScreenStyles';
import { COLORS } from '../styles/GlobalStyles';

const WelcomeScreen = ({ navigation, route }) => {
  // --- CORREÇÃO: Evita crash se params vier vazio ---
  const { idusuario, nome } = route.params || { idusuario: null, nome: 'Usuário' };

  const [sidebarOpen, setSidebarOpen] = useState(false);

  const features = [
    { icon: 'wrench-outline', text: 'Abrir e gerenciar ordens de serviço.' },
    { icon: 'settings-outline', text: 'Acompanhar o status de consertos de hardware e software.' },
  ];

  return (
    <SafeAreaView style={WelcomeStyle.container}>
      <Header
        showBackButton={false}
        onProfilePress={() => navigation.navigate('Profile')}
      />

      <View style={WelcomeStyle.content}>
        <TouchableOpacity
          style={WelcomeStyle.menuButton}
          onPress={() => setSidebarOpen(true)}
        >
          <Ionicons name="menu" size={28} color="#000" />
        </TouchableOpacity>

        <View style={WelcomeStyle.welcomeCard}>
          <Text style={WelcomeStyle.welcomeTitle}>Bem-vindo, {nome}!</Text>
          
          <View style={WelcomeStyle.infoBox}>
            <Text style={WelcomeStyle.infoText}>
              Bem-vindo ao sistema da <Text style={WelcomeStyle.highlight}>InterFix</Text>.
            </Text>
            
            <Text style={WelcomeStyle.sectionTitle}>Aqui você pode:</Text>
            
            {features.map((feature, index) => (
              <View key={index} style={WelcomeStyle.featureItem}>
                <Ionicons name={feature.icon} size={20} color="#00BFFF" />
                <Text style={WelcomeStyle.featureText}>{feature.text}</Text>
              </View>
            ))}
            
            <Text style={WelcomeStyle.navigationHint}>
              Use o menu à esquerda para navegar entre os módulos disponíveis.
            </Text>
          </View>
        </View>
      </View>

      <Sidebar
        isOpen={sidebarOpen}
        onClose={() => setSidebarOpen(false)}
        navigation={navigation}
        userId={idusuario} // --- CORREÇÃO: Passando ID via prop ---
      />
    </SafeAreaView>
  );
};

export default WelcomeScreen;