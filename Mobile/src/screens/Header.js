import React from 'react';
import { View, Text, TouchableOpacity, Image } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import Headerstyles from '../styles/HeaderStyles';

const Header = ({ showBackButton, title, onBackPress, onProfilePress }) => {
  return (
    <View style={Headerstyles.container}>
      <View style={Headerstyles.leftSection}>
        {showBackButton && (
          <TouchableOpacity onPress={onBackPress} style={Headerstyles.backButton}>
            <Ionicons name="arrow-back" size={24} color="#000" />
            <Text style={Headerstyles.backText}>Voltar</Text>
          </TouchableOpacity>
        )}
        {!showBackButton && (
          <Text style={Headerstyles.logo}>
            Inter<Text style={Headerstyles.logoHighlight}>Fix</Text>
          </Text>
        )}
      </View>
      
      {title && <Text style={Headerstyles.title}>{title}</Text>}
      
      <TouchableOpacity onPress={onProfilePress} style={Headerstyles.profileButton}>
        <Ionicons name="person-circle-outline" size={32} color="#000" />
      </TouchableOpacity>
    </View>
  );
};

export default Header;