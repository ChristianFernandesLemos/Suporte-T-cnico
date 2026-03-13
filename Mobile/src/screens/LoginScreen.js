import React, { useState } from 'react';
import {
  View, Text, TextInput, TouchableOpacity, Alert, 
  KeyboardAvoidingView, Platform, ScrollView, ActivityIndicator
} from 'react-native';
import { StatusBar } from 'expo-status-bar';
import { GlobalStyles, COLORS, SPACING, getResponsiveDimensions } from '../styles/GlobalStyles';
import { sendToN8n } from '../../components/requisicoes'; 

const LoginScreen = ({ navigation }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);

  const handleLogin = async () => {
    const cleanEmail = email.trim();
    const cleanPassword = password.trim();

    if (!cleanEmail || !cleanPassword) {
      Alert.alert('Erro', 'Preencha e-mail e senha.');
      return;
    }
    
    setLoading(true);

    // Chama nossa função blindada
    const resultado = await sendToN8n(null, null, 3, cleanEmail, cleanPassword);
    
    setLoading(false);

    // Agora a lógica é muito simples:
    if (resultado.success && resultado.usuario) {
        // Se entrou aqui, já sabemos que tem ID e é Ativo (validado no outro arquivo)
        navigation.navigate('WelcomeScreen', { 
           idusuario: resultado.usuario.Id_usuario, 
           nome: resultado.usuario.nome,
        }); 
    } else {
        // Mostra o erro exato que definimos no requisicoes.js
        Alert.alert("Acesso Negado", resultado.erro || "Tente novamente.");
    }
  };
  
  const { cardMaxWidth, horizontalPadding } = getResponsiveDimensions();

  return (
    <KeyboardAvoidingView 
      style={GlobalStyles.container} 
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      keyboardVerticalOffset={Platform.OS === 'ios' ? 64 : 0}
    >
      <StatusBar style="light" backgroundColor={COLORS.black} />
      <ScrollView 
        style={{ flex: 1, backgroundColor: COLORS.background.dark }}
        contentContainerStyle={[GlobalStyles.scrollContainer, { padding: horizontalPadding, flexGrow: 1 }]}
        keyboardShouldPersistTaps="handled"
      >
        <View style={[GlobalStyles.card, { maxWidth: cardMaxWidth }]}>
          <Text style={GlobalStyles.title}>InterFix Login</Text>
          <Text style={GlobalStyles.subtitle}>Entre para acessar suas configurações.</Text>
          
          <View style={GlobalStyles.inputContainer}>
            <Text style={GlobalStyles.inputLabel}>E-mail</Text>
            <TextInput
              style={GlobalStyles.input}
              placeholder="Digite seu e-mail"
              placeholderTextColor={COLORS.text.muted}
              value={email}
              onChangeText={setEmail}
              keyboardType="email-address"
              autoCapitalize="none"
            />
          </View>

          <View style={GlobalStyles.inputContainer}>
            <Text style={GlobalStyles.inputLabel}>Senha</Text>
            <TextInput
              style={GlobalStyles.input}
              placeholder="Digite sua senha"
              placeholderTextColor={COLORS.text.muted}
              value={password}
              onChangeText={setPassword}
              secureTextEntry
            />
          </View>

          <TouchableOpacity 
            style={[GlobalStyles.primaryButton, { marginTop: SPACING.sm }]} 
            onPress={handleLogin}
            disabled={loading}
          >
            {loading ? <ActivityIndicator color="#FFF" /> : <Text style={GlobalStyles.primaryButtonText}>Entrar</Text>}
          </TouchableOpacity>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
};

export default LoginScreen;