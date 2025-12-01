import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  Alert,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  ActivityIndicator,
} from 'react-native';
import { Picker } from '@react-native-picker/picker';
import { StatusBar } from 'expo-status-bar';

// --- IMPORTS ---
import { sendToN8nToIa } from '../../components/ia';
import { COLORS } from '../styles/GlobalStyles'; 
import stylesnewTicket from '../styles/NewTicketScreenStyle';
import Header from './Header'; // <--- IMPORTADO AQUI

const NewTicketScreen = ({ navigation, route }) => {
  const [currentStep, setCurrentStep] = useState(1);
  const [loading, setLoading] = useState(false);
  
  const { idusuario } = route.params || {};

  const [formData, setFormData] = useState({
    title: '',
    employeeName: '',
    email: '',
    category: 'software', 
    description: '',
    affectedPeople: '', 
    blocksWork: '',
    suggestedPriority: '',       
    userPriorityReason: '',     
    ticketId: null,              
  });

  const updateFormData = (field, value) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const resetForm = () => {
      setFormData({
          title: '', employeeName: '', email: '', category: 'software', 
          description: '', affectedPeople: '', blocksWork: '',
          suggestedPriority: '', userPriorityReason: '', ticketId: null,
      });
      setCurrentStep(1);
  };
  
  const enviarChamadoParaN8N = async (dados) => {
    if (!idusuario) {
        Alert.alert("Erro", "ID do usuário não identificado. Faça login novamente.");
        return null;
    }
    return await sendToN8nToIa(
      idusuario, dados.title, dados.employeeName, dados.email, dados.category,          
      dados.description, dados.affectedPeople, dados.blocksWork,      
      dados.userPriorityReason, dados.status            
    );
  };

  const validateCurrentStep = () => {
    switch (currentStep) {
        case 1:
          if (!formData.title.trim() || !formData.employeeName.trim() || !formData.email.trim() || !formData.description.trim()) {
           Alert.alert('Campos obrigatórios', 'Por favor, preencha todos os campos da etapa 1.');
           return false;
          }
          break;
        case 2:
          if (!formData.affectedPeople) {
             Alert.alert('Seleção necessária', 'Por favor, selecione quem está sendo afetado.');
             return false;
          }
          break;
        case 3:
          if (!formData.blocksWork) {
             Alert.alert('Seleção necessária', 'Por favor, informe se impede o trabalho.');
             return false;
          }
          break; 
        case 5: 
          if (!formData.userPriorityReason.trim()) {
            Alert.alert('Justificativa necessária', 'Por favor, explique o motivo da discordância.');
            return false;
          }
          break;
      }
      return true;
  };

  const nextStep = async () => {
    if (!validateCurrentStep()) return;

    if (currentStep === 3) {
        setLoading(true);
        try {
            const dataToSend = { ...formData, status: 1 }; 
            const n8nResponse = await enviarChamadoParaN8N(dataToSend);
            setLoading(false);

            const prioridadeRecebida = n8nResponse?.prioridade || n8nResponse?.suggestedPriority;

            if (n8nResponse && prioridadeRecebida) {
                setFormData(prev => ({ 
                    ...prev, 
                    suggestedPriority: prioridadeRecebida,
                    ticketId: n8nResponse.ticketId || prev.ticketId 
                }));
                setCurrentStep(4);
            } else {
                Alert.alert('Aviso', 'Não foi possível obter a sugestão da IA. Seguindo para revisão manual.');
                setFormData(prev => ({ ...prev, suggestedPriority: 'Média' })); 
                setCurrentStep(4);
            }
        } catch (error) {
            setLoading(false);
            Alert.alert('Erro de Conexão', 'Falha ao comunicar com a IA.');
        }
    } else {
      setCurrentStep(prev => prev + 1);
    }
  };

  const prevStep = () => {
    if (currentStep > 1) setCurrentStep(prev => prev - 1);
  };
  
  const handleFinish = async () => {
    setLoading(true);
    try {
        const dataToSend = { ...formData, status: 2 };
        const n8nResponse = await enviarChamadoParaN8N(dataToSend);
        setLoading(false);

        if (n8nResponse) {
            Alert.alert('Chamado Criado!', `O chamado foi registrado com sucesso.`,
                [{ text: 'OK', onPress: () => { resetForm(); navigation.goBack(); }, }]
            );
        } else {
            Alert.alert('Erro', 'O servidor não confirmou a finalização.');
        }
    } catch (error) {
        setLoading(false);
        Alert.alert('Erro de Conexão', 'Falha ao registrar o chamado final.');
    }
  };

  // --- RENDERIZADORES DOS STEPS (Mantidos iguais, omitidos para brevidade se não houver alteração interna) ---
  const renderStep1 = () => (
    <View style={stylesnewTicket.stepContainer}>
      <Text style={stylesnewTicket.stepTitle}>Novo Chamado</Text>
      <View style={stylesnewTicket.inputContainer}>
        <Text style={stylesnewTicket.inputLabel}>Título:</Text>
        <TextInput style={stylesnewTicket.input} value={formData.title} placeholder='O Título do seu chamado' placeholderTextColor="rgba(184, 181, 181, 0.5)" onChangeText={(value) => updateFormData('title', value)} />
      </View>
      <View style={stylesnewTicket.inputContainer}>
        <Text style={stylesnewTicket.inputLabel}>Nome:</Text>
        <TextInput style={stylesnewTicket.input} value={formData.employeeName} placeholder='Digite o seu nome completo' placeholderTextColor="rgba(184, 181, 181, 0.5)" onChangeText={(value) => updateFormData('employeeName', value)} />
      </View>
      <View style={stylesnewTicket.inputContainer}>
        <Text style={stylesnewTicket.inputLabel}>E-mail:</Text>
        <TextInput style={stylesnewTicket.input} value={formData.email} placeholder='Digite o seu email' placeholderTextColor="rgba(184, 181, 181, 0.5)" onChangeText={(value) => updateFormData('email', value)} keyboardType="email-address" autoCapitalize="none" />
      </View>
      <View style={stylesnewTicket.inputContainer}>
        <Text style={stylesnewTicket.inputLabel}>Categoria:</Text>
        <View style={stylesnewTicket.pickerContainer}>
          <Picker selectedValue={formData.category} style={stylesnewTicket.picker} onValueChange={(value) => updateFormData('category', value)}>
            <Picker.Item label="Software" value="software" /><Picker.Item label="Hardware" value="hardware" /><Picker.Item label="Rede" value="rede"/><Picker.Item label="Outros" value="outro"/>
          </Picker>
        </View>
      </View>
      <View style={stylesnewTicket.inputContainer}>
        <Text style={stylesnewTicket.inputLabel}>Descrição:</Text>
        <TextInput style={[stylesnewTicket.input, stylesnewTicket.textArea]} placeholder="Descreva o problema com detalhes..." placeholderTextColor="rgba(184, 181, 181, 0.5)" value={formData.description} onChangeText={(value) => updateFormData('description', value)} multiline numberOfLines={4} />
      </View>
      <TouchableOpacity style={stylesnewTicket.primaryButton} onPress={nextStep} disabled={loading}><Text style={stylesnewTicket.primaryButtonText}>Avançar</Text></TouchableOpacity>
    </View>
  );

  const renderStep2 = () => (
    <View style={stylesnewTicket.stepContainer}>
      <Text style={stylesnewTicket.stepTitle}>Abrangência</Text>
      <Text style={stylesnewTicket.questionText}>Quem está sendo afetado pelo problema?</Text>
      <View style={stylesnewTicket.optionsContainer}>
        <TouchableOpacity style={[stylesnewTicket.optionButton, formData.affectedPeople === 'eu' && stylesnewTicket.optionButtonSelected]} onPress={() => updateFormData('affectedPeople', 'eu')}><Text style={[stylesnewTicket.optionText, formData.affectedPeople === 'eu' && stylesnewTicket.optionTextSelected]}>Apenas Eu</Text></TouchableOpacity>
        <TouchableOpacity style={[stylesnewTicket.optionButton, formData.affectedPeople === 'equipe' && stylesnewTicket.optionButtonSelected]} onPress={() => updateFormData('affectedPeople', 'equipe')}><Text style={[stylesnewTicket.optionText, formData.affectedPeople === 'equipe' && stylesnewTicket.optionTextSelected]}>Minha Equipe</Text></TouchableOpacity>
        <TouchableOpacity style={[stylesnewTicket.optionButton, formData.affectedPeople === 'empresa' && stylesnewTicket.optionButtonSelected]} onPress={() => updateFormData('affectedPeople', 'empresa')}><Text style={[stylesnewTicket.optionText, formData.affectedPeople === 'empresa' && stylesnewTicket.optionTextSelected]}>Toda a Empresa</Text></TouchableOpacity>
      </View>
      <View style={stylesnewTicket.buttonRow}>
        <TouchableOpacity style={stylesnewTicket.secondaryButton} onPress={prevStep} disabled={loading}><Text style={stylesnewTicket.secondaryButtonText}>Voltar</Text></TouchableOpacity>
        <TouchableOpacity style={stylesnewTicket.primaryButton} onPress={nextStep} disabled={loading}><Text style={stylesnewTicket.primaryButtonText}>Avançar</Text></TouchableOpacity>
      </View>
    </View>
  );

  const renderStep3 = () => (
    <View style={stylesnewTicket.stepContainer}>
      <Text style={stylesnewTicket.stepTitle}>Impacto</Text>
      <Text style={stylesnewTicket.questionText}>O problema impede totalmente o trabalho?</Text>
      <View style={stylesnewTicket.checkboxContainer}>
        <TouchableOpacity style={stylesnewTicket.checkboxRow} onPress={() => updateFormData('blocksWork', 'nao')}><View style={[stylesnewTicket.checkbox, formData.blocksWork === 'nao' && stylesnewTicket.checkboxSelected]}>{formData.blocksWork === 'nao' && <Text style={stylesnewTicket.checkmark}>✓</Text>}</View><Text style={stylesnewTicket.checkboxLabel}>Não, consigo trabalhar</Text></TouchableOpacity>
        <TouchableOpacity style={stylesnewTicket.checkboxRow} onPress={() => updateFormData('blocksWork', 'sim')}><View style={[stylesnewTicket.checkbox, formData.blocksWork === 'sim' && stylesnewTicket.checkboxSelected]}>{formData.blocksWork === 'sim' && <Text style={stylesnewTicket.checkmark}>✓</Text>}</View><Text style={stylesnewTicket.checkboxLabel}>Sim, estou parado</Text></TouchableOpacity>
      </View>
      <View style={stylesnewTicket.buttonRow}>
        <TouchableOpacity style={stylesnewTicket.secondaryButton} onPress={prevStep} disabled={loading}><Text style={stylesnewTicket.secondaryButtonText}>Voltar</Text></TouchableOpacity>
        <TouchableOpacity style={stylesnewTicket.primaryButton} onPress={nextStep} disabled={loading}>{loading ? <ActivityIndicator size="small" color="#fff" /> : <Text style={stylesnewTicket.primaryButtonText}>Analisar Prioridade</Text>}</TouchableOpacity>
      </View>
    </View>
  );

  const renderStep4 = () => (
    <View style={stylesnewTicket.stepContainer}>
      <Text style={stylesnewTicket.stepTitle}>Sugestão da IA</Text>
      <View style={{ marginVertical: 20 }}>
          <Text style={stylesnewTicket.questionText}>O sistema analisou seu chamado e sugeriu:</Text>
          <Text style={{ fontSize: 24, fontWeight: 'bold', color: COLORS.primary, textAlign: 'center', marginTop: 10 }}>{formData.suggestedPriority ? formData.suggestedPriority.toUpperCase() : 'AGUARDANDO...'}</Text>
      </View>
      <Text style={stylesnewTicket.questionText}>Você concorda com esta prioridade?</Text>
      <View style={stylesnewTicket.buttonRow}>
        <TouchableOpacity style={stylesnewTicket.secondaryButton} onPress={prevStep} disabled={loading}><Text style={stylesnewTicket.secondaryButtonText}>Voltar</Text></TouchableOpacity>
        <TouchableOpacity style={[stylesnewTicket.secondaryButton, { backgroundColor: '#FF6B6B', borderColor: '#FF6B6B' }]} onPress={() => setCurrentStep(5)} disabled={loading}><Text style={stylesnewTicket.secondaryButtonText}>Discordo</Text></TouchableOpacity>
        <TouchableOpacity style={stylesnewTicket.primaryButton} onPress={() => setCurrentStep(6)} disabled={loading}><Text style={stylesnewTicket.primaryButtonText}>Concordo</Text></TouchableOpacity>
      </View>
    </View>
  );

  const renderStep5 = () => (
    <View style={stylesnewTicket.stepContainer}>
      <Text style={stylesnewTicket.stepTitle}>Ajuste Manual</Text>
      <Text style={stylesnewTicket.questionText}>Por favor, explique por que você discorda da sugestão:</Text>
      <View style={stylesnewTicket.inputContainer}>
        <Text style={stylesnewTicket.inputLabel}>Motivo:</Text>
        <TextInput 
          style={[stylesnewTicket.input, stylesnewTicket.textArea]} 
          placeholder="Descreva o motivo..." 
          placeholderTextColor="rgba(184, 181, 181, 0.5)" 
          value={formData.userPriorityReason} 
          onChangeText={(value) => updateFormData('userPriorityReason', value)} 
          multiline 
          numberOfLines={3} 
        />
      </View>
      <View style={stylesnewTicket.buttonRow}>
        <TouchableOpacity style={stylesnewTicket.secondaryButton} onPress={prevStep} disabled={loading}><Text style={stylesnewTicket.secondaryButtonText}>Voltar</Text></TouchableOpacity>
        <TouchableOpacity style={stylesnewTicket.primaryButton} onPress={nextStep} disabled={loading}><Text style={stylesnewTicket.primaryButtonText}>Revisar</Text></TouchableOpacity>
      </View>
    </View>
  );

  const renderStep6 = () => (
    <View style={stylesnewTicket.stepContainer}>
      <Text style={stylesnewTicket.stepTitle}>Revisão Final</Text>
      <View style={{ backgroundColor: '#f0f0f0', padding: 15, borderRadius: 8, marginVertical: 15 }}>
          <Text style={{ fontSize: 16, marginBottom: 5 }}>Título: <Text style={{ fontWeight: 'bold' }}>{formData.title}</Text></Text>
          <Text style={{ fontSize: 16, marginBottom: 5 }}>Prioridade Final: <Text style={{ fontWeight: 'bold', color: COLORS.primary }}>{formData.userPriority || formData.suggestedPriority}</Text></Text>
          {formData.userPriorityReason ? (<Text style={{ fontSize: 14, fontStyle: 'italic', marginTop: 5, color: '#555' }}>Obs: Usuário discordou da prioridade sugerida. Motivo: "{formData.userPriorityReason}"</Text>) : (<Text style={{ fontSize: 14, color: '#555', marginTop: 5 }}>Prioridade sugerida pela IA aceita.</Text>)}
      </View>
      <View style={stylesnewTicket.buttonRow}>
        <TouchableOpacity style={stylesnewTicket.secondaryButton} onPress={prevStep} disabled={loading}><Text style={stylesnewTicket.secondaryButtonText}>Voltar</Text></TouchableOpacity>
        <TouchableOpacity style={stylesnewTicket.primaryButton} onPress={handleFinish} disabled={loading}>{loading ? <ActivityIndicator size="small" color="#fff" /> : <Text style={stylesnewTicket.primaryButtonText}>Confirmar e Enviar</Text>}</TouchableOpacity>
      </View>
    </View>
  );

  const renderCurrentStep = () => {
    switch (currentStep) {
      case 1: return renderStep1();
      case 2: return renderStep2();
      case 3: return renderStep3();
      case 4: return renderStep4();
      case 5: return renderStep5(); 
      case 6: return renderStep6();  
      default: return renderStep1();
    }
  };

  return (
    <KeyboardAvoidingView 
      style={stylesnewTicket.container} 
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
    >
      <StatusBar style="light" backgroundColor="#000" />
      
      {/* --- HEADER ADICIONADO AQUI --- */}
      <Header 
        showBackButton={true} 
        onBackPress={() => navigation.goBack()}
        // Se quiser um título centralizado, descomente abaixo:
        // title="Novo Chamado"
      />

      <ScrollView 
        contentContainerStyle={stylesnewTicket.scrollContainer}
        keyboardShouldPersistTaps="handled"
        showsVerticalScrollIndicator={false}
      >
        <View style={stylesnewTicket.card}>
          {renderCurrentStep()}
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
};

export default NewTicketScreen;